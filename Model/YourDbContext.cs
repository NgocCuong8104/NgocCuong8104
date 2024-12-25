using WpfApp1.Model;
using System.Data.Entity;

public class YourDbContext : DbContext
{
    public DbSet<user> Users { get; set; }
    public DbSet<log> Logs { get; set; }
    public DbSet<Huyen> Huyens { get; set; }
    public DbSet<Xa> Xas { get; set; }
    public DbSet<Co_so> CoSos { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<quy_mo> quy_mo { get; set; }
    public DbSet<Con_giong_vat_nuoi> ConGiongVatNuois { get; set; }
    public DbSet<animal> Animals { get; set; }
    public DbSet<Tinh_phoi_uu_trung_aptrung> TinhPhois { get; set; }
    public DbSet<Gen_dv> Gens { get; set; }
    public DbSet<Loai_hinh_user> LoaiHinhs { get; set; }
    public DbSet<Chung_nhan> ChungNhans { get; set; }


    // Constructor của DbContext
    public YourDbContext() : base("name=DbContext") // Dùng chuỗi kết nối "DbContext" từ config
    {
    }
}
