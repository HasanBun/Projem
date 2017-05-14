using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETicaretMVC.Models;
using ETicaretMVC.DAL;
using System.Data;

namespace ETicaretMVC.Controllers
{
    public class UrunController : Controller
    {
        SatisContext veritabani = new SatisContext();

        public ActionResult Anasayfa()
        {
            List<Urun> urunler = veritabani.Urunler.ToList();
            return View(urunler);
        }
        public ActionResult Detay(int id)
        {
            Urun urun = (from u in veritabani.Urunler where u.Id == id select u).FirstOrDefault();
            return View(urun);
        }
        public ActionResult Kategori(int id)
        {
            string kategoriAdi = (from k in veritabani.Kategoriler where k.Id == id select k.Ad).FirstOrDefault();
            ViewBag.Title = kategoriAdi + " Kategorisindeki Ürünler";
            List<Urun> urunler = (from u in veritabani.Urunler where u.KategoriId == id select u).ToList();

            return View(urunler);
        }
        public ActionResult SepeteEkle(int id)
        {
            Urun urun = (from u in veritabani.Urunler where u.Id == id select u).FirstOrDefault();
            DataTable spt = new DataTable();

            if (Session["sepet"] != null)
            {
                spt = (DataTable)Session["sepet"];
            }
            else
            {
                spt.Columns.Add("Id");
                spt.Columns.Add("Adı");
                spt.Columns.Add("Fiyat");
            }

            DataRow dr = spt.NewRow();
            dr["Id"] = urun.Id;
            dr["Adı"] = urun.Ad;
            dr["Fiyat"] = urun.Fiyat;
            spt.Rows.Add(dr);

            Session["sepet"] = spt;
            ViewBag.Toplam = SepetToplam();

            return View(spt);
        }
        public double SepetToplam()
        {
            double toplam = 0;
            if (Session["sepet"] != null)
            {

                DataTable dt = new DataTable();
                dt = (DataTable)Session["sepet"];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    toplam += Convert.ToDouble(dt.Rows[i]["Fiyat"].ToString());
                }

            }
            return toplam;
        }
        public ActionResult SepetOnay()
        {
            string kullanıcı = Session["Kullanici"].ToString();
            Kullanici kn = (from a in veritabani.Kullanicilar where a.Email == kullanıcı select a).FirstOrDefault();
            return View(kn);
        }
        public ActionResult SiparisOnay(int id)
        {
            Kullanici klln = (from u in veritabani.Kullanicilar where u.Id == id select u).FirstOrDefault();
            ViewBag.Mesaj = "Sayın" + klln.Ad + " " + klln.Soyad + " " + "Siparişiniz Onaylanmıştır.";
            DataTable dt = new DataTable();
            dt = (DataTable)Session["sepet"];
            int urunId;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                urunId = int.Parse(dt.Rows[i]["Id"].ToString());
                Urun urunler = (from u in veritabani.Urunler where u.Id == urunId select u).FirstOrDefault();

                Siparis siparislerim = new Siparis();
                siparislerim.Ad = urunler.Ad;
                siparislerim.Fiyat = urunler.Fiyat;
                siparislerim.KullaniciEmail = klln.Email;
                veritabani.SaveChanges();
            }
            return View();
        }
    }
}