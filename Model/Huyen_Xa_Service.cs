using WpfApp1.Model;
using System;
using System.Linq;

public class Huyen_Xa_Service
{
    public YourDbContext dbContext; // Thay đổi quyền truy cập thành public

    public Huyen_Xa_Service()
    {
        dbContext = new YourDbContext();
    }

    // In ra danh sách huyện
    public void InDanhSachHuyen()
    {
        var huyens = dbContext.Huyens.ToList();
        foreach (var huyen in huyens)
        {
            Console.WriteLine($"ID: {huyen.id_Huyen}, Name: {huyen.name}");
        }
    }

    // In ra danh sách xã
    public void InDanhSachXa()
    {
        var xas = dbContext.Xas.ToList();
        foreach (var xa in xas)
        {
            Console.WriteLine($"ID: {xa.id_xa}, Name: {xa.name}, HuyenID: {xa.id_Huyen}");
        }
    }

    // Tìm huyện theo ID
    public Huyen TimHuyen(int id)
    {
        return dbContext.Huyens.Find(id);
    }

    // Tìm xã theo ID
    public Xa TimXa(int id)
    {
        return dbContext.Xas.Find(id);
    }

    // Thêm huyện mới
    public void ThemHuyen(int id, string name)
    {
        var huyen = new Huyen { id_Huyen = id, name = name };
        dbContext.Huyens.Add(huyen);
        dbContext.SaveChanges();
    }

    // Thêm xã mới
    public void ThemXa(int id, string name, int huyenId)
    {
        var xa = new Xa { id_xa = id, name = name, id_Huyen = huyenId };
        dbContext.Xas.Add(xa);
        dbContext.SaveChanges();
    }

    // Cập nhật thông tin huyện
    public void CapNhatThongTinHuyen(int id, string newName)
    {
        var huyen = dbContext.Huyens.Find(id);
        if (huyen != null)
        {
            huyen.name = newName;
            dbContext.SaveChanges();
        }
    }

    // Cập nhật thông tin xã
    public void CapNhatThongTinXa(int id, string newName, int newHuyenId)
    {
        var xa = dbContext.Xas.Find(id);
        if (xa != null)
        {
            xa.name = newName;
            xa.id_Huyen = newHuyenId;
            dbContext.SaveChanges();
        }
    }
}
