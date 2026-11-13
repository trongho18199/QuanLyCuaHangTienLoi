using ConvenienceStore.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;

namespace ConvenienceStore.Data.Repositories
{
    
    public class UserListItem
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }

        public string StatusText { get { return IsActive ? "Hoạt động" : "Đã khóa"; } }
    }

    public class UserRepository
    {
        
        
        
        
        public User FindByCredentials(string username, string password)
        {
            string hash = PasswordHasher.Hash(password);

            using (var db = new CSDbContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<User>(u => u.Role);
                db.LoadOptions = options;

                return db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == hash);
            }
        }

        

        public OpResult ChangePassword(int userId, string oldPassword, string newPassword)
        {
            using (var db = new CSDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return OpResult.Fail("Không tìm thấy tài khoản.");

                if (user.PasswordHash != PasswordHasher.Hash(oldPassword))
                    return OpResult.Fail("Mật khẩu hiện tại không đúng.");

                user.PasswordHash = PasswordHasher.Hash(newPassword);
                db.SubmitChanges();
                return OpResult.Ok();
            }
        }

        

        public List<UserListItem> GetAll(string keyword)
        {
            keyword = (keyword ?? "").Trim();

            using (var db = new CSDbContext())
            {
                var query = from u in db.Users
                            join r in db.Roles on u.RoleId equals r.Id
                            where keyword == ""
                                  || u.Username.Contains(keyword)
                                  || (u.FullName != null && u.FullName.Contains(keyword))
                            orderby u.Username
                            select new UserListItem
                            {
                                Id = u.Id,
                                Username = u.Username,
                                FullName = u.FullName,
                                RoleId = u.RoleId,
                                RoleName = r.Name,
                                IsActive = u.IsActive
                            };
                return query.ToList();
            }
        }

        public OpResult Add(string username, string password, string fullName, int roleId, bool isActive)
        {
            using (var db = new CSDbContext())
            {
                if (db.Users.Any(u => u.Username == username))
                    return OpResult.Fail("Tên đăng nhập đã tồn tại.");
                if (!db.Roles.Any(r => r.Id == roleId))
                    return OpResult.Fail("Nhóm quyền không tồn tại.");

                db.Users.InsertOnSubmit(new User
                {
                    Username = username,
                    PasswordHash = PasswordHasher.Hash(password),
                    FullName = fullName,
                    IsActive = isActive,
                    RoleId = roleId
                });
                db.SubmitChanges();
                return OpResult.Ok();
            }
        }

        
        public OpResult Update(int userId, string fullName, int roleId, bool isActive, int actingUserId)
        {
            using (var db = new CSDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return OpResult.Fail("Không tìm thấy tài khoản.");
                if (!db.Roles.Any(r => r.Id == roleId)) return OpResult.Fail("Nhóm quyền không tồn tại.");

                bool changesAccess = roleId != user.RoleId || !isActive;

                if (changesAccess && userId == actingUserId)
                    return OpResult.Fail("Không thể đổi nhóm quyền hoặc khóa chính tài khoản đang đăng nhập.");
                if (changesAccess && IsLastActiveAdmin(db, user))
                    return OpResult.Fail("Phải còn ít nhất một tài khoản Admin đang hoạt động.");

                user.FullName = fullName;
                user.RoleId = roleId;
                user.IsActive = isActive;
                db.SubmitChanges();
                return OpResult.Ok();
            }
        }

        public OpResult ResetPassword(int userId, string newPassword)
        {
            using (var db = new CSDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return OpResult.Fail("Không tìm thấy tài khoản.");

                user.PasswordHash = PasswordHasher.Hash(newPassword);
                db.SubmitChanges();
                return OpResult.Ok();
            }
        }

        public OpResult Delete(int userId, int actingUserId)
        {
            if (userId == actingUserId)
                return OpResult.Fail("Không thể xóa chính tài khoản đang đăng nhập.");

            using (var db = new CSDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return OpResult.Fail("Không tìm thấy tài khoản.");
                if (IsLastActiveAdmin(db, user))
                    return OpResult.Fail("Phải còn ít nhất một tài khoản Admin đang hoạt động.");

                try
                {
                    db.Users.DeleteOnSubmit(user);
                    db.SubmitChanges();
                    return OpResult.Ok();
                }
                catch (SqlException ex)
                {
                    
                    if (ex.Number == 547)
                        return OpResult.Fail("Tài khoản này đã phát sinh dữ liệu (hóa đơn, phiếu nhập...) nên không thể xóa. Hãy khóa tài khoản thay vì xóa.");
                    throw;
                }
            }
        }

        
        private static bool IsLastActiveAdmin(CSDbContext db, User user)
        {
            int? adminRoleId = db.Roles.Where(r => r.Name == RoleRepository.AdminRoleName)
                                       .Select(r => (int?)r.Id).FirstOrDefault();
            if (adminRoleId == null || user.RoleId != adminRoleId.Value || !user.IsActive)
                return false;

            int adminId = adminRoleId.Value;
            return !db.Users.Any(u => u.Id != user.Id && u.RoleId == adminId && u.IsActive);
        }
    }
}
