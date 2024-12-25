using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;

namespace WpfApp1.Model
{
    public class Coso_Service
    {
        private readonly YourDbContext _context;

        public Coso_Service(YourDbContext context)
        {
            _context = context;
        }

        public void AddCoso(Co_so newCoso)
        {
            if (newCoso == null)
            {
                throw new ArgumentNullException(nameof(newCoso), "Dữ liệu Co_so không hợp lệ.");
            }

            _context.CoSos.Add(newCoso);
            _context.SaveChanges();
        }

        public IQueryable<Co_so> GetAllCoso()
        {
            try
            {
                return _context.CoSos
                               .Include(c => c.animal)
                               .Include(c => c.Con_giong_vat_nuoi)
                               .Include(c => c.Gen_dv)
                               .Include(c => c.Huyen)
                               .Include(c => c.Xa)
                               .Include(c => c.Loai_hinh_user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi truy vấn dữ liệu cơ sở: {ex.Message}");
            }
        }



        public Co_so GetCosoDetails(int id)
        {
            return _context.CoSos
                           .Include(c => c.Huyen)
                           .Include(c => c.Xa)
                           .Include(c => c.Loai_hinh_user)
                           .Include(c => c.Con_giong_vat_nuoi)
                           .Include(c => c.animal)
                           .Include(c => c.Gen_dv)
                           .Include(c => c.Tinh_phoi_uu_trung_aptrung)
                           .Include(c => c.Chung_nhan)
                           .FirstOrDefault(c => c.id == id);
        }
       
        public List<Chung_nhan> GetAllChungNhans()
        {
            return _context.ChungNhans.ToList();
        }
        public void DeleteCoso(int id)
        {
            try
            {
                var coso = _context.CoSos.FirstOrDefault(c => c.id == id);
                if (coso == null)
                {
                    throw new Exception($"Không tìm thấy Co_so với ID: {id}");
                }

                _context.Users.Where(u => u.id_co_so == id)
                              .ToList()
                              .ForEach(u => u.id_co_so = null);

                _context.CoSos.Remove(coso);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa Co_so: {ex.Message}");
            }
        }

        public string UpdateCoso(Co_so updatedCoso)
        {
            if (updatedCoso == null)
            {
                return "Dữ liệu Co_so không hợp lệ.";
            }

            try
            {
                var existingCoso = _context.CoSos.Find(updatedCoso.id);
                if (existingCoso == null)
                {
                    return "Không tìm thấy Co_so cần cập nhật.";
                }

                existingCoso.name = updatedCoso.name;
                existingCoso.id_xa = updatedCoso.id_xa;
                existingCoso.id_huyen = updatedCoso.id_huyen;
                existingCoso.id_animal = updatedCoso.id_animal;
                existingCoso.id_gen = updatedCoso.id_gen;
                existingCoso.id_thuc_an_chan_nuoi = updatedCoso.id_thuc_an_chan_nuoi;
                existingCoso.id_tinh_phoi = updatedCoso.id_tinh_phoi;
                existingCoso.id_loai_hinh = updatedCoso.id_loai_hinh;

                _context.Entry(existingCoso).State = EntityState.Modified;
                _context.SaveChanges();
                return "Cập nhật thành công.";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi cập nhật Co_so: {ex.Message}";
            }
        }

        public bool IsCosoNameExists(string name, int? excludeId = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Tên cơ sở không hợp lệ.", nameof(name));
            }

            var query = _context.CoSos.Where(c => c.name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.id != excludeId.Value);
            }

            return query.Any();
        }

        public List<Huyen> GetAllHuyens()
        {
            return _context.Huyens.ToList();
        }

        public List<Xa> GetXasByHuyenId(int huyenId)
        {
            return _context.Xas.Where(x => x.id_Huyen == huyenId).ToList();
        }

        public IQueryable<Co_so> SearchCoso(
          int? id = null,
          string name = null,
          int? idConGiongVatNuoi = null,
          int? idAnimal = null,
          int? idTinhPhoi = null,
          int? idLoaiHinh = null,
          int? idUserDungDau = null,
          int? idXa = null,
          int? idHuyen = null,
          int? idGen = null,
          bool? idThucAnChanNuoi = null,
          int? idChungNhan = null)
        {
            var query = _context.CoSos.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(c => c.id == id.Value);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.name.Contains(name));
            }

            if (idConGiongVatNuoi.HasValue)
            {
                query = query.Where(c => c.id_con_giong_vat_nuoi == idConGiongVatNuoi.Value);
            }

            if (idAnimal.HasValue)
            {
                query = query.Where(c => c.id_animal == idAnimal.Value);
            }

            if (idTinhPhoi.HasValue)
            {
                query = query.Where(c => c.id_tinh_phoi == idTinhPhoi.Value);
            }

            if (idLoaiHinh.HasValue)
            {
                query = query.Where(c => c.id_loai_hinh == idLoaiHinh.Value);
            }

            if (idUserDungDau.HasValue)
            {
                query = query.Where(c => c.id_user_dung_dau == idUserDungDau.Value);
            }

            if (idXa.HasValue)
            {
                query = query.Where(c => c.id_xa == idXa.Value);
            }

            if (idHuyen.HasValue)
            {
                query = query.Where(c => c.id_huyen == idHuyen.Value);
            }

            if (idGen.HasValue)
            {
                query = query.Where(c => c.id_gen == idGen.Value);
            }

            if (idThucAnChanNuoi.HasValue)
            {
                query = query.Where(c => c.id_thuc_an_chan_nuoi == idThucAnChanNuoi.Value);
            }

            if (idChungNhan.HasValue)
            {
                query = query.Where(c => c.id_chung_nhan == idChungNhan.Value);
            }

            return query.Include(c => c.Huyen)
                        .Include(c => c.Xa)
                        .Include(c => c.Con_giong_vat_nuoi)
                        .Include(c => c.animal)
                        .Include(c => c.Gen_dv);
        }

        public user GetUserByFacilityId(int facilityId)
        {
            return _context.Users.FirstOrDefault(u => u.id_co_so == facilityId);
        }


        public List<Con_giong_vat_nuoi> GetAllConGiongVatNuoi()
        {
            return _context.ConGiongVatNuois.ToList();
        }
        public List<animal> GetAllAnimals()
    {
        return _context.Animals.ToList();
    }

    public List<Con_giong_vat_nuoi> GetAllConGiongVatNuois()
    {
        return _context.ConGiongVatNuois.ToList();
    }

    public List<Tinh_phoi_uu_trung_aptrung> GetAllTinhPhois()
    {
        return _context.TinhPhois.ToList();
    }

    public List<Gen_dv> GetAllGens()
    {
        return _context.Gens.ToList();
    }

  
    public List<Loai_hinh_user> GetAllLoaiHinhs()
    {
        return _context.LoaiHinhs.ToList();
    }
    }
}
