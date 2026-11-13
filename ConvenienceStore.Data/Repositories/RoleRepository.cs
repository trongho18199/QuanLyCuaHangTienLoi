using ConvenienceStore.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConvenienceStore.Data.Repositories
{
    
    public class RoleListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserCount { get; set; }

        public override string ToString()
        {
            return Name + "  (" + UserCount + " tài khoản)";
        }
    }

    public class RoleRepository
    {
        
        public const string AdminRoleName = "Admin";

        
        public static readonly string[] AdminLockedCodes = { "mnuUserManagement", "mnuRoleManagement" };

        public List<RoleListItem> GetAll()
        {
            using (var db = new CSDbContext())
            {
                return (from r in db.Roles
                        orderby r.Name
                        select new RoleListItem { Id = r.Id, Name = r.Name, UserCount = r.Users.Count })
                       .ToList();
            }
        }

        public string GetName(int roleId)
        {
            using (var db = new CSDbContext())
            {
                return db.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefault();
            }
        }

        public OpResult Add(string name)
        {
            name = (name ?? "").Trim();
            if (name.Length == 0) return OpResult.Fail("Vui lòng nhập tên nhóm quyền.");
            if (name.Length > 50) return OpResult.Fail("Tên nhóm quyền tối đa 50 ký tự.");

            using (var db = new CSDbContext())
            {
                if (db.Roles.Any(r => r.Name == name))
                    return OpResult.Fail("Tên nhóm quyền đã tồn tại.");

                db.Roles.InsertOnSubmit(new Role { Name = name });
                db.SubmitChanges();
                return OpResult.Ok();
            }
        }

        public OpResult Rename(int roleId, string newName)
        {
            newName = (newName ?? "").Trim();
            if (newName.Length == 0) return OpResult.Fail("Vui lòng nhập tên nhóm quyền.");
            if (newName.Length > 50) return OpResult.Fail("Tên nhóm quyền tối đa 50 ký tự.");

            using (var db = new CSDbContext())
            {
                var role = db.Roles.FirstOrDefault(r => r.Id == roleId);
                if (role == null) return OpResult.Fail("Không tìm thấy nhóm quyền.");
                if (role.Name == AdminRoleName) return OpResult.Fail("Không thể đổi tên nhóm Admin.");
                if (db.Roles.Any(r => r.Name == newName && r.Id != roleId))
                    return OpResult.Fail("Tên nhóm quyền đã tồn tại.");

                role.Name = newName;
                db.SubmitChanges();
                return OpResult.Ok();
            }
        }

        public OpResult Delete(int roleId)
        {
            using (var db = new CSDbContext())
            {
                var role = db.Roles.FirstOrDefault(r => r.Id == roleId);
                if (role == null) return OpResult.Fail("Không tìm thấy nhóm quyền.");
                if (role.Name == AdminRoleName) return OpResult.Fail("Không thể xóa nhóm Admin.");
                if (db.Users.Any(u => u.RoleId == roleId))
                    return OpResult.Fail("Nhóm này đang có tài khoản. Hãy chuyển các tài khoản sang nhóm khác trước khi xóa.");

                
                db.RolePermissions.DeleteAllOnSubmit(db.RolePermissions.Where(p => p.RoleId == roleId));
                db.SubmitChanges();

                db.Roles.DeleteOnSubmit(role);
                db.SubmitChanges();
                return OpResult.Ok();
            }
        }

        
        public List<string> GetPermissionCodes(int roleId)
        {
            using (var db = new CSDbContext())
            {
                return db.RolePermissions.Where(p => p.RoleId == roleId)
                         .Select(p => p.FunctionCode).ToList();
            }
        }

        
        public OpResult SavePermissions(int roleId, IEnumerable<string> functionCodes)
        {
            using (var db = new CSDbContext())
            {
                var role = db.Roles.FirstOrDefault(r => r.Id == roleId);
                if (role == null) return OpResult.Fail("Không tìm thấy nhóm quyền.");

                var wanted = new HashSet<string>(functionCodes ?? new string[0]);
                if (role.Name == AdminRoleName)
                    foreach (var code in AdminLockedCodes) wanted.Add(code);

                var existing = db.RolePermissions.Where(p => p.RoleId == roleId).ToList();
                var existingCodes = new HashSet<string>(existing.Select(p => p.FunctionCode));

                db.RolePermissions.DeleteAllOnSubmit(existing.Where(p => !wanted.Contains(p.FunctionCode)));
                db.RolePermissions.InsertAllOnSubmit(
                    wanted.Where(c => !existingCodes.Contains(c))
                          .Select(c => new RolePermission { RoleId = roleId, FunctionCode = c }));

                db.SubmitChanges();
                return OpResult.Ok();
            }
        }
    }
}
