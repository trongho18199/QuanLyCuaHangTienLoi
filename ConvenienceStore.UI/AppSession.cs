using ConvenienceStore.Data;
using ConvenienceStore.Data.Repositories;

namespace ConvenienceStore.UI
{
    
    public static class AppSession
    {
        private static User _currentUser;
        private static string _roleName;

        public static User CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; _roleName = null; }
        }

        
        
        
        
        public static string RoleName
        {
            get
            {
                if (_roleName == null && _currentUser != null)
                    _roleName = new RoleRepository().GetName(_currentUser.RoleId) ?? string.Empty;
                return _roleName ?? string.Empty;
            }
        }

        public static bool IsAdmin
        {
            get { return RoleName == RoleRepository.AdminRoleName; }
        }
    }
}
