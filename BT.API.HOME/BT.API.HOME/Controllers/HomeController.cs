﻿using System;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using BT.API.HOME.Models;
using OracleInternal.Secure.Network;

namespace BT.API.HOME.Controllers
{
    [RoutePrefix("api/home")]
    public class HomeController : ApiController
    {
        public IHttpActionResult GetListMerchanedise(decimal pagenumber , decimal pagesize)
        {
            List<VatTuModel> lstVatTu = new List<VatTuModel>();
            VatTuDTO vattu = new VatTuDTO(lstVatTu);
            string table_XNT = CommonService.GET_TABLE_NAME_NGAYHACHTOAN_CSDL_ORACLE();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if(connection.State == ConnectionState.Open)
                {
                    decimal P_PAGENUMBER = pagenumber;
                    decimal P_PAGESIZE = pagesize;
                    OracleCommand command = new OracleCommand();
                    command.Connection = connection;
                    command.InitialLONGFetchSize = 1000;
                    command.CommandText = string.Format(@"SELECT * FROM ( SELECT a.*, rownum r__ FROM ( SELECT vt.MAVATTU , vt.TENVATTU , vt.GIABANLEVAT ,vt.Avatar, vt.PATH_IMAGE , vt.IMAGE , xnt.TONCUOIKYSL  FROM V_VATTU_GIABAN vt LEFT JOIN " + table_XNT + " xnt ON vt.MAVATTU = xnt.MAVATTU  WHERE vt.MADONVI ='DV1-CH1' AND xnt.MAKHO ='DV1-CH1-KBL' ORDER BY vt.I_CREATE_DATE DESC ) a WHERE rownum < ((" + P_PAGENUMBER + " * " + P_PAGESIZE + ") + 1 )  )  WHERE r__ >= (((" + P_PAGENUMBER + "-1) * " + P_PAGESIZE + ") + 1)");
                    command.CommandType = CommandType.Text;
                    try
                    {
                        OracleDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            decimal dongia ,soluong = 0;
                            while (reader.Read())
                            {
                                VatTuModel temp = new VatTuModel();
                                temp.MaVatTu = reader["MAVATTU"].ToString();
                                temp.TenVatTu = reader["TENVATTU"].ToString();
                                decimal.TryParse(reader["GIABANLEVAT"].ToString(), out dongia);
                                temp.DonGia = dongia;
                                decimal.TryParse(reader["TONCUOIKYSL"].ToString(), out soluong);
                                temp.SoTon = soluong;
                                string HinhAnh = reader["IMAGE"].ToString();
                                string[] lstAnh = HinhAnh.Split(',');
                                temp.HinhAnh = new List<string>();
                                temp.Avatar = (byte[])reader["Avatar"];
                                string Path = reader["PATH_IMAGE"].ToString();
                                for (int i = 0; i < lstAnh.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(lstAnh[i]))
                                    {
                                        temp.HinhAnh.Add(Path + lstAnh[i]);
                                    }
                                }
                                lstVatTu.Add(temp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = @"SELECT COUNT(*) TOTALITEM FROM V_VATTU_GIABAN vt WHERE vt.MADONVI ='DV1-CH1'";
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        decimal  totalitem = 0;
                        while (dataReader.Read())
                        {
                            decimal.TryParse(dataReader["TOTALITEM"].ToString(), out totalitem);
                            vattu.ItemTotal = totalitem;
                            vattu.PageSize = pagesize;
                            vattu.PageNumber = pagenumber;
                        }
                    }
                }
            }
            return Ok(vattu);
        }

        public IHttpActionResult GetListMerchanediseByCategory(decimal pagenumber, decimal pagesize,string merchanedisetype)
        {
            List<VatTuModel> lstVatTu = new List<VatTuModel>();
            VatTuDTO vattu = new VatTuDTO(lstVatTu);
            string table_XNT = CommonService.GET_TABLE_NAME_NGAYHACHTOAN_CSDL_ORACLE();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    decimal P_PAGENUMBER = pagenumber;
                    decimal P_PAGESIZE = pagesize;
                    OracleCommand command = new OracleCommand();
                    command.Connection = connection;
                    command.InitialLONGFetchSize = 1000;
                    command.CommandText = string.Format(@"SELECT * FROM ( SELECT a.*, rownum r__ FROM ( SELECT vt.MAVATTU , vt.TENVATTU , vt.GIABANLEVAT ,vt.Avatar, vt.PATH_IMAGE , vt.IMAGE , xnt.TONCUOIKYSL  FROM V_VATTU_GIABAN vt LEFT JOIN " + table_XNT + " xnt ON vt.MAVATTU = xnt.MAVATTU  WHERE vt.MADONVI ='DV1-CH1' AND xnt.MAKHO ='DV1-CH1-KBL' AND vt.MANHOMVATTU='" + merchanedisetype + "' OR vt.MALOAIVATTU = '"+ merchanedisetype + "' ORDER BY vt.I_CREATE_DATE DESC ) a WHERE rownum < ((" + P_PAGENUMBER + " * " + P_PAGESIZE + ") + 1 )  )  WHERE r__ >= (((" + P_PAGENUMBER + "-1) * " + P_PAGESIZE + ") + 1)");
                    command.CommandType = CommandType.Text;
                    try
                    {
                        OracleDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            decimal dongia, soluong = 0;
                            while (reader.Read())
                            {
                                VatTuModel temp = new VatTuModel();
                                temp.MaVatTu = reader["MAVATTU"].ToString();
                                temp.TenVatTu = reader["TENVATTU"].ToString();
                                decimal.TryParse(reader["GIABANLEVAT"].ToString(), out dongia);
                                temp.DonGia = dongia;
                                decimal.TryParse(reader["TONCUOIKYSL"].ToString(), out soluong);
                                temp.SoTon = soluong;
                                string HinhAnh = reader["IMAGE"].ToString();
                                string[] lstAnh = HinhAnh.Split(',');
                                temp.HinhAnh = new List<string>();
                                temp.Avatar = (byte[])reader["Avatar"];
                                string Path = reader["PATH_IMAGE"].ToString();
                                for (int i = 0; i < lstAnh.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(lstAnh[i]))
                                    {
                                        temp.HinhAnh.Add(Path + lstAnh[i]);
                                    }
                                }
                                lstVatTu.Add(temp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = @"SELECT COUNT(*) TOTALITEM FROM V_VATTU_GIABAN vt WHERE vt.MADONVI ='DV1-CH1' AND vt.MANHOMVATTU='" + merchanedisetype + "' OR vt.MALOAIVATTU = '" + merchanedisetype + "'";
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        decimal totalitem = 0;
                        while (dataReader.Read())
                        {
                            decimal.TryParse(dataReader["TOTALITEM"].ToString(), out totalitem);
                            vattu.ItemTotal = totalitem;
                            vattu.PageSize = pagesize;
                            vattu.PageNumber = pagenumber;
                        }
                    }
                }
            }
            return Ok(vattu);
        }

        public IHttpActionResult GetDetailMerchanedise(string mavattu , string madonvi)
        {
            VatTuDetail result = new VatTuDetail();
            string table_XNT = CommonService.GET_TABLE_NAME_NGAYHACHTOAN_CSDL_ORACLE();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"SELECT vt.MAVATTU , vt.TENVATTU,vt.MASIZE ,vt.TITLE ,vt.MADONVI, vt.GIABANLEVAT,vt.TENNHACUNGCAP ,vt.Avatar, vt.PATH_IMAGE , vt.IMAGE , xnt.TONCUOIKYSL  FROM V_VATTU_GIABAN vt LEFT JOIN " + table_XNT+" xnt ON vt.MAVATTU = xnt.MAVATTU WHERE vt.MADONVI ='"+ madonvi + "' AND xnt.MAKHO ='DV1-CH1-KBL' AND vt.MAVATTU = :mavattu";
                    cmd.Parameters.Add("mavattu", OracleDbType.NVarchar2, 50).Value = mavattu;
                    try
                    {
                        OracleDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                decimal dongia, soluong = 0;
                                result.MaVatTu = reader["MAVATTU"].ToString();
                                result.TenVatTu = reader["TENVATTU"].ToString();
                                decimal.TryParse(reader["GIABANLEVAT"].ToString(), out dongia);
                                result.DonGia = dongia;
                                decimal.TryParse(reader["TONCUOIKYSL"].ToString(), out soluong);
                                result.SoTon = soluong;
                                string HinhAnh = reader["IMAGE"].ToString();
                                string[] lstAnh = HinhAnh.Split(',');
                                result.HinhAnhs = new List<string>();
                                result.Avatar = (byte[])reader["Avatar"];
                                string Path = reader["PATH_IMAGE"].ToString();
                                result.MaDonVi = reader["MADONVI"].ToString();
                                for (int i = 0; i < lstAnh.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(lstAnh[i]))
                                    {
                                        result.HinhAnhs.Add(Path + lstAnh[i]);
                                    }
                                }
                                result.NhaCungCap = reader["TENNHACUNGCAP"].ToString();
                                result.MoTa = reader["TITLE"].ToString();
                                string[] sizes = reader["MASIZE"].ToString().Split(',');
                                result.Size = sizes;
                            }
                        }
                    }catch(Exception ex) { }
                }
            }
            return Ok(result);
        }

        public IHttpActionResult GetListMerchanediseType(string madonvi)
        {
            List<MerchanediseType> lstMerchanediseType = new List<MerchanediseType>();
            using (
                OracleConnection connection =
                    new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    OracleCommand command = new OracleCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"SELECT * FROM DM_LOAIVATTU WHERE UNITCODE = :madonvi";
                    command.Parameters.Add("madonvi", OracleDbType.NVarchar2,10).Value = madonvi;
                    OracleDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            MerchanediseType merchanediseType = new MerchanediseType();
                            merchanediseType.MaLoaiVatTu = reader["MALOAIVATTU"].ToString();
                            merchanediseType.TenLoaiVatTu = reader["TENLOAIVT"].ToString();
                            merchanediseType.UnitCode = reader["UNITCODE"].ToString();
                            lstMerchanediseType.Add(merchanediseType);
                        }
                    }
                }
            }
            return Ok(lstMerchanediseType);
        }

        public IHttpActionResult GetAllGroupMerchanedise(string unitcode)
        {
            List<NhomVatTuModel> lstNhomVatTu = new List<NhomVatTuModel>();
            using (
                OracleConnection connection =
                    new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    OracleCommand command = new OracleCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"SELECT * FROM DM_NHOMVATTU WHERE UNITCODE = :madonvi ";
                    command.Parameters.Add("madonvi", OracleDbType.NVarchar2, 10).Value = unitcode;
                    OracleDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            NhomVatTuModel temp = new NhomVatTuModel();
                            temp.MALOAIVATTU = reader["MALOAIVATTU"].ToString();
                            temp.MANHOMVATTU = reader["MANHOMVTU"].ToString();
                            temp.TENNHOMVATTU = reader["TENNHOMVT"].ToString();
                            temp.MADONVI = reader["UNITCODE"].ToString();
                            temp.MACHA = reader["MACHA"].ToString();
                            lstNhomVatTu.Add(temp);
                        }
                    }
                }
            }
            return Ok(lstNhomVatTu);
        }

        public IHttpActionResult GetMerchanediseByCode(string mavattuselect,string madonvi)
        {
            VatTuViewCart result = new VatTuViewCart();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"SELECT vt.AVATAR,vt.TENHANG,vt.GIABANLEVAT FROM V_VATTU_GIABAN vt WHERE vt.MAVATTU=:mavattu AND vt.UNITCODE = :madonvi";
                    cmd.Parameters.Add("mavattu", OracleDbType.NVarchar2, 50).Value = mavattuselect;
                    cmd.Parameters.Add("madonvi", OracleDbType.NVarchar2, 50).Value = madonvi;
                    try
                    {
                        OracleDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                decimal dongia= 0;
                                result.TenVatTu = reader["TENHANG"].ToString();
                                decimal.TryParse(reader["GIABANLEVAT"].ToString(), out dongia);
                                result.GiaBanLeVat = dongia;
                                result.Avatar = (byte[])reader["Avatar"];
                                result.SoLuong = 0;
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            return Ok(result);
        }

        public IHttpActionResult GetListMerchanediseKhuyenMai(decimal pagenumber, decimal pagesize,string makho,string madonvi)
        {
            List<VatTuModel> lstVatTu = new List<VatTuModel>();
            VatTuDTO vattu = new VatTuDTO(lstVatTu);
            string table_XNT = CommonService.GET_TABLE_NAME_NGAYHACHTOAN_CSDL_ORACLE();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    decimal P_PAGENUMBER = pagenumber;
                    decimal P_PAGESIZE = pagesize;
                    OracleCommand command = new OracleCommand();
                    command.Connection = connection;
                    command.InitialLONGFetchSize = 1000;
                    command.CommandText = string.Format(@"SELECT * FROM ( SELECT a.*, rownum r__ FROM ( SELECT km.MACHUONGTRINH,km.TUNGAY,DENNGAY,km.TUGIO,km.DENGIO,km.MAVATTU,km.SOLUONG,km.TYLEKHUYENMAICHILDREN AS TYLE,km.GIATRIKHUYENMAICHILDREN AS GIATRI,vt.GIABANLEVAT,vt.AVATAR,vt.TENHANG,xnt.TONCUOIKYSL SOTON FROM V_VATTU_GIABAN vt RIGHT JOIN V_CHUONGTRINH_KHUYENMAI km ON vt.MAVATTU = km.MAVATTU LEFT JOIN "+ table_XNT + " xnt ON xnt.MAVATTU= km.MAVATTU WHERE km.UNITCODE = '"+ madonvi + "' AND km.TRANGTHAI = 10 AND vt.UNITCODE ='"+ madonvi + "' AND xnt.MAKHO ='"+makho+"' AND km.TUNGAY >=TO_DATE(SYSDATE,'DD/MM/YY') AND km.DENNGAY <= TO_DATE(SYSDATE,'DD/MM/YY') ORDER BY vt.I_CREATE_DATE DESC ) a WHERE rownum < ((" + P_PAGENUMBER + " * " + P_PAGESIZE + ") + 1 )  )  WHERE r__ >= (((" + P_PAGENUMBER + "-1) * " + P_PAGESIZE + ") + 1)");
                    command.CommandType = CommandType.Text;
                    try
                    {
                        OracleDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            decimal dongia, soluong ,khuyenmai,tyle= 0;
                            while (reader.Read())
                            {
                                VatTuModel temp = new VatTuModel();
                                temp.MaVatTu = reader["MAVATTU"].ToString();
                                temp.TenVatTu = reader["TENVATTU"].ToString();
                                decimal.TryParse(reader["GIABANLEVAT"].ToString(), out dongia);
                                temp.DonGia = dongia;
                                decimal.TryParse(reader["TONCUOIKYSL"].ToString(), out soluong);
                                temp.SoTon = soluong;
                                string HinhAnh = reader["IMAGE"].ToString();
                                string[] lstAnh = HinhAnh.Split(',');
                                temp.HinhAnh = new List<string>();
                                temp.Avatar = (byte[])reader["Avatar"];
                                string Path = reader["PATH_IMAGE"].ToString();
                                decimal.TryParse(reader["GIATRI"].ToString(), out khuyenmai);
                                temp.DonGiaKhuyenMai = khuyenmai;
                                decimal.TryParse(reader["TYLE"].ToString(), out tyle);
                                temp.TyLeKhuyeMai = tyle;
                                for (int i = 0; i < lstAnh.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(lstAnh[i]))
                                    {
                                        temp.HinhAnh.Add(Path + lstAnh[i]);
                                    }
                                }
                                lstVatTu.Add(temp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = @"SELECT COUNT(*) TOTALITEM FROM V_CHUONGTRINH_KHUYENMAI vt WHERE vt.UNITCODE ='" + madonvi+ "' AND vt.TUNGAY >=TO_DATE(SYSDATE,'DD/MM/YY') AND vt.DENNGAY <= TO_DATE(SYSDATE,'DD/MM/YY') ";
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        decimal totalitem = 0;
                        while (dataReader.Read())
                        {
                            decimal.TryParse(dataReader["TOTALITEM"].ToString(), out totalitem);
                            vattu.ItemTotal = totalitem;
                            vattu.PageSize = pagesize;
                            vattu.PageNumber = pagenumber;
                        }
                    }
                }
            }
            return Ok(vattu);
        }

        [HttpPost]
        [Route("RegisKhachHang")]
        public IHttpActionResult RegisKhachHang(KhachHangModel obj)
        {
            string newID = CommonService.GET_NEW_ID("KH");
            ObjectResult dataResult = new ObjectResult();
            string data = "";
            using (OracleConnection connection =new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    OracleCommand command = new OracleCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"INSERT INTO DM_KHACHHANG (ID,MAKH,TENKH,TENKHAC,DIACHI,TRANGTHAI,DIENTHOAI,EMAIL,NGAYSINH,I_CREATE_DATE,UNITCODE)
                                            VALUES (:ID,:MAKH,:TENKH,:TENKHAC,:DIACHI,:TRANGTHAI,:DIENTHOAI,:EMAIL,:NGAYSINH,:CREATEDATE,:UNITCODE)";
                    command.Parameters.Add("ID", OracleDbType.NVarchar2, 50).Value = Guid.NewGuid();
                    command.Parameters.Add("MAKH", OracleDbType.NVarchar2, 50).Value = "KH";
                    command.Parameters.Add("TENKH", OracleDbType.NVarchar2, 500).Value = obj.TenKH;
                    command.Parameters.Add("TENKHAC", OracleDbType.NVarchar2, 500).Value = obj.TenKhac;
                    command.Parameters.Add("DIACHI", OracleDbType.NVarchar2, 500).Value = obj.DiaChi;
                    //command.Parameters.Add("TINH", OracleDbType.NVarchar2, 50).Value = obj.TinhTP;
                    command.Parameters.Add("TRANGTHAI", OracleDbType.Decimal).Value = 10;
                    command.Parameters.Add("DIENTHOAI", OracleDbType.NVarchar2,50).Value = obj.DienThoai;
                    command.Parameters.Add("EMAIL", OracleDbType.NVarchar2, 100).Value = obj.Email;
                    command.Parameters.Add("NGAYSINH", OracleDbType.Date).Value = obj.NgaySinh;
                    command.Parameters.Add("CREATEDATE", OracleDbType.Date).Value = DateTime.Now;
                    command.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = obj.MaDonVi;
                    try
                    {
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = connection;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText =
                                @"INSERT INTO AU_NGUOIDUNG (ID , USERNAME, PASSWORD ,MANHANVIEN,SODIENTHOAI,TRANGTHAI,I_CREATE_DATE,UNITCODE)
                                   VALUES (:ID , :USERNAME , :PASSWORD , :MANHANVIEN , :SODIENTHOAI , :TRANGTHAI , :CREATEDATE ,:UNITCODE)";
                            cmd.Parameters.Add("ID",OracleDbType.NVarchar2,50).Value = Guid.NewGuid();
                            cmd.Parameters.Add("USERNAME", OracleDbType.NVarchar2, 50).Value = obj.DienThoai;
                            cmd.Parameters.Add("PASSWORD", OracleDbType.NVarchar2, 100).Value =CommonService.MD5Hash(obj.MatKhau);
                            cmd.Parameters.Add("MANHANVIEN", OracleDbType.NVarchar2, 50).Value = "KH";
                            cmd.Parameters.Add("SODIENTHOAI", OracleDbType.NVarchar2, 50).Value = obj.DienThoai;
                            cmd.Parameters.Add("TRANGTHAI", OracleDbType.Decimal).Value = 10;
                            cmd.Parameters.Add("CREATEDATE", OracleDbType.Date).Value = DateTime.Now;
                            cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = obj.MaDonVi;
                            result = cmd.ExecuteNonQuery();
                            if (result > 0)
                            {
                                data = "Thêm mới thành công ";
                                dataResult.Message = data;
                                dataResult.Result = true;
                            }
                        }
                        else
                        {
                            data = " Thêm mới thất bại vui lòng kiểm tra lại !";
                            dataResult.Message = data;
                            dataResult.Result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        data = " Thêm mới thất bại vui lòng kiểm tra lại !";
                        dataResult.Message = data;
                        dataResult.Result = false;
                    }
                }
            }

            return Ok(dataResult);
        }

        [HttpGet]
        public IHttpActionResult Login(string username,string pass, string donvi)
        {
            ObjectResult dataResult = new ObjectResult();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    string password = CommonService.MD5Hash(pass);
                    OracleCommand command = new OracleCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"SELECT * FROM AU_NGUOIDUNG WHERE USERNAME = '"+ username + "' AND PASSWORD='"+ password + "' AND UNITCODE ='"+ donvi + "'";
                    try
                    {
                        OracleDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                dataResult.Result = true;
                                dataResult.Message = "Đăng nhập thành công !";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        dataResult.Result = false;
                        dataResult.Message = "Đăng nhập thất bại!";
                    }
                }
            }
            return Ok(dataResult);
        }

        [HttpGet]
        public IHttpActionResult GetUserByPhone(string sodienthoai,string unitcode2)
        {
            KhachHangModel dataResult = new KhachHangModel();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    OracleCommand command = new OracleCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"SELECT * FROM DM_KHACHHANG WHERE DIENTHOAI= :dienthoai AND UNITCODE=:unitcode";
                    command.Parameters.Add("dienthoai", OracleDbType.NVarchar2, 50).Value = sodienthoai;
                    command.Parameters.Add("unitcode", OracleDbType.NVarchar2, 50).Value = unitcode2;
                    try
                    {
                        OracleDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                dataResult.TenKH = reader["TENKH"].ToString();
                                dataResult.DienThoai = reader["DIENTHOAI"].ToString();
                                dataResult.DiaChi = reader["DIACHI"].ToString();
                                dataResult.MaDonVi = reader["UNITCODE"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            return Ok(dataResult);
        }

        [HttpPost]
        [Route("CheckOut")]
        public IHttpActionResult CheckOut(ObjectCartModel data)
        {
            ObjectResult dataResult = new ObjectResult();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    DateTime time = DateTime.Now;
                    string soPhieu = data.SDTNN + "_" + time.Minute + time.Hour + time.Day;
                    string soPhieuPK = data.SDTNN + "_" + time.Minute + time.Hour + time.Day + "_" + data.UNITCODE;
                    OracleTransaction txn = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    OracleCommand command = new OracleCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"INSERT INTO NVDATHANG (ID,SOPHIEU,MAHD,SOPHIEUPK,LOAI,NGAY,MAKHACHHANG,THANHTIENSAUVAT,TRANGTHAI,TENNN,SDTNN,DIACHINN,TRANGTHAITT,ISBANBUON,I_CREATE_DATE,UNITCODE,SOPHIEUCON)
                                            VALUES (:ID,:SOPHIEU,:MAHD,:SOPHIEUPK,:LOAI,:NGAY,:MAKHACHHANG,:THANHTIENSAUVAT,:TRANGTHAI,:TENNN,:SDTNN,:DIACHINN,:TRANGTHAITT,:ISBANBUON,:I_CREATE_DATE,:UNITCODE,:SOPHIEUCON)";
                    command.Parameters.Add("ID", OracleDbType.NVarchar2, 50).Value = Guid.NewGuid();
                    command.Parameters.Add("SOPHIEU", OracleDbType.NVarchar2, 50).Value = soPhieu;
                    command.Parameters.Add("MAHD",OracleDbType.NVarchar2,50).Value = soPhieu;
                    command.Parameters.Add("SOPHIEUPK", OracleDbType.NVarchar2, 50).Value = soPhieuPK;
                    command.Parameters.Add("LOAI", OracleDbType.Decimal).Value = 10;
                    command.Parameters.Add("NGAY", OracleDbType.Date).Value = time;
                    command.Parameters.Add("MAKHACHHANG", OracleDbType.NVarchar2, 50).Value = data.SDTNN;
                    command.Parameters.Add("THANHTIENSAUVAT", OracleDbType.NVarchar2, 50).Value =data.THANHTIENSAUVAT;
                    command.Parameters.Add("TRANGTHAI", OracleDbType.Decimal).Value = 10;
                    command.Parameters.Add("TENNN", OracleDbType.NVarchar2, 50).Value = data.TENNN;
                    command.Parameters.Add("SDTNN", OracleDbType.NVarchar2, 50).Value = data.SDTNN;
                    command.Parameters.Add("DIACHINN", OracleDbType.NVarchar2, 50).Value = data.DIACHINN;
                    command.Parameters.Add("TRANGTHAITT", OracleDbType.Decimal).Value = 0;
                    command.Parameters.Add("ISBANBUON", OracleDbType.Decimal).Value = 0;
                    command.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = time;
                    command.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = data.UNITCODE;
                    command.Parameters.Add("SOPHIEUCON", OracleDbType.Decimal).Value = data.SOPHIEUCON;
                    try
                    {
                        int result = command.ExecuteNonQuery();
                        if(result > 0)
                        {
                            int itemp =0, itemp2= 0;
                            data.Details.ForEach(x =>
                            {
                                OracleCommand cmd = new OracleCommand();
                                cmd.Connection = connection;
                                cmd.CommandType = CommandType.Text;
                                itemp2++;
                                cmd.CommandText = @"INSERT INTO NVDATHANGCHITIET (ID,SOPHIEU,SOPHIEUPK,MAHD,MAHANG,TENHANG,SOLUONG,DONGIA,THANHTIEN)
                                                    VALUES (:ID,:SOPHIEU,:SOPHIEUPK,:MAHD,:MAHANG,:TENHANG,:SOLUONG,:DONGIA,:THANHTIEN)";
                                cmd.Parameters.Add("ID", OracleDbType.NVarchar2, 50).Value = Guid.NewGuid();
                                cmd.Parameters.Add("SOPHIEU", OracleDbType.NVarchar2, 50).Value = soPhieu;
                                cmd.Parameters.Add("SOPHIEUPK", OracleDbType.NVarchar2, 50).Value = soPhieuPK;
                                cmd.Parameters.Add("MAHD", OracleDbType.NVarchar2, 50).Value = soPhieu;
                                cmd.Parameters.Add("MAHANG", OracleDbType.NVarchar2, 50).Value = x.MAHANG;
                                cmd.Parameters.Add("TENHANG", OracleDbType.NVarchar2, 50).Value = x.TENHANG;
                                cmd.Parameters.Add("SOLUONG", OracleDbType.Decimal).Value = x.SOLUONG;
                                cmd.Parameters.Add("DONGIA", OracleDbType.Decimal).Value = x.DONGIA ;
                                cmd.Parameters.Add("THANHTIEN", OracleDbType.Decimal).Value = x.DONGIA *x.SOLUONG;
                                int resultDetail = cmd.ExecuteNonQuery();
                                if(resultDetail > 0)
                                {
                                    itemp++;
                                }
                            });
                            if(itemp == itemp2)
                            {
                                dataResult.Message = "Thanh toán thành công !";
                                dataResult.Result = true;
                                txn.Commit();
                            }
                            else
                            {
                                txn.Rollback();
                                dataResult.Message = "Thanh toán thất bại vui lòng kiểm tra lại !";
                                dataResult.Result = false;
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        dataResult.Message = "Thanh toán thất bại vui lòng kiểm tra lại !";
                        dataResult.Result = false;
                        txn.Rollback();
                    }
                }
            }
            return Ok(dataResult);
        }

        public HttpResponseMessage Put()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent("PUT: Test message")
            };
        }
    }
}
