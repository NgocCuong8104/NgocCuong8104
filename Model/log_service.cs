using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace WpfApp1.Model
{
    public class LogService
    {
        private readonly YourDbContext _dbContext;

        public LogService(YourDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string GetIpAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "IP not found.";
            }
            catch (Exception ex)
            {
                return $"Error retrieving IP: {ex.Message}";
            }
        }

        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        public void AddLog(int idLoaiHinhTacDong, int idUser, string duLieuTacDong)
        {
            try
            {
                var newLog = new log
                {
                    id_loai_hinh_tac_dong = idLoaiHinhTacDong,
                    id_user = idUser,
                    time = GetCurrentTime(),
                    du_lieu_tac_dong = duLieuTacDong
                };

                _dbContext.Logs.Add(newLog);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding log: {ex.Message}");
            }
        }
    }
}
