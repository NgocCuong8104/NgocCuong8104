using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Windows;

namespace WpfApp1.Model
{
    internal class User_Service
    {
        private readonly YourDbContext _context;

        public User_Service(YourDbContext context)
        {
            _context = context;
        }

        public bool CheckUserExists(string userName)
        {
            return _context.Users.Any(u => u.user_name == userName);
        }

        public bool CheckPassword(int userId, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.id == userId);
            return user != null && user.password == password;
        }

        public void AddUser(user newUser)
        {
            _context.Users.Add(newUser);
            _context.SaveChanges();
        }

        public void DeleteUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.id == userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public IQueryable<user> GetAllUsers()
        {
            return _context.Users
                .Include(u => u.Huyen)
                .Include(u => u.Xa)
                .Include(u => u.Role)
                .Include(u => u.quy_mo); // Bao gồm bảng Quy mô
        }


        public void UpdateUser(user updatedUser)
        {
            if (updatedUser == null)
            {
                ShowMessage("Dữ liệu người dùng không hợp lệ. Cập nhật thất bại.", "Lỗi", MessageBoxImage.Error);
                return;
            }

            var user = _context.Users.FirstOrDefault(u => u.id == updatedUser.id);

            if (user != null)
            {
                if (IsXaBelongsToHuyen(updatedUser.id_xa, updatedUser.id_huyen))
                {
                    // Cập nhật các trường của người dùng từ updatedUser
                    UpdateUserFields(user, updatedUser);
                    _context.SaveChanges();
                    ShowMessage("Cập nhật người dùng thành công.", "Thành công", MessageBoxImage.Information);
                }
                else
                {
                    ShowMessage("id_xa không thuộc về id_huyen này. Cập nhật thất bại.", "Lỗi", MessageBoxImage.Warning);
                }
            }
            else
            {
                ShowMessage("Không tìm thấy người dùng với id đã cho. Cập nhật thất bại.", "Lỗi", MessageBoxImage.Warning);
            }
        }


        private bool IsXaBelongsToHuyen(int? idXa, int? idHuyen)
        {
            var xa = _context.Xas.FirstOrDefault(x => x.id_xa == idXa);
            return xa != null && xa.id_Huyen == idHuyen;
        }

        private void UpdateUserFields(user user, user updatedUser)
        {
            user.user_name = updatedUser.user_name;
            user.password = updatedUser.password;
            user.email = updatedUser.email;
            user.sdt = updatedUser.sdt;
            user.status = updatedUser.status;
            user.id_xa = updatedUser.id_xa;
            user.id_huyen = updatedUser.id_huyen;
        }

        private void ShowMessage(string message, string caption, MessageBoxImage icon)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, icon);
        }

        public List<Role> GetAllRoles()
        {
            return _context.Roles.ToList();
        }

        public string GetUserRole(string userName, string password)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.user_name == userName && u.password == password);

            return user?.Role?.name;
        }

        public user ValidateUser(string userName, string password)
        {
            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.user_name == userName && u.password == password);
        }
        public user GetUserById(int userId)
        {
            return _context.Users
                .AsNoTracking() // Bảo đảm lấy dữ liệu mới nhất
                .Include(u => u.Huyen)
                .Include(u => u.Xa)
                .Include(u => u.Role)
                .FirstOrDefault(u => u.id == userId);
        }



        public List<Huyen> GetAllHuyens()
        {
            return _context.Huyens.ToList();
        }

        public List<Xa> GetXasByHuyenId(int huyenId)
        {
            return _context.Xas.Where(x => x.id_Huyen == huyenId).ToList();
        }
        public List<quy_mo> GetAllQuyMo()
        {
            return _context.quy_mo.ToList();
        }

        public IQueryable<user> SearchUsers(int? id = null, string userName = null, string sdt = null, string email = null, int? idXa = null, int? idHuyen = null)
        {
            var query = _context.Users.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(u => u.id == id.Value);
            }

            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(u => u.user_name.Contains(userName));
            }

            if (!string.IsNullOrEmpty(sdt))
            {
                query = query.Where(u => u.sdt.Contains(sdt));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.email.Contains(email));
            }

            if (idXa.HasValue)
            {
                query = query.Where(u => u.id_xa == idXa.Value);
            }

            if (idHuyen.HasValue)
            {
                query = query.Where(u => u.id_huyen == idHuyen.Value);
            }

            return query.Include(u => u.Huyen)
                        .Include(u => u.Xa);
        }
    }
}
