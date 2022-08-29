using Domainssl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Domainssl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SSLController : Controller
    {
        String strconnection = "Data Source=152.67.29.26,1565;initial catalog=augusta_intern;User ID=lokesh;Password=WvsEnQ4Z;";
        [SwaggerOperation(Summary = "Gets all the domain name  with expiry date.")]
        [HttpGet("GetAllDates")]
        [Authorize]
        public List<Dates> GetAllDates()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {

                List<Dates> test = new List<Dates>();
                Dates row1 = new Dates();


                SqlDataAdapter da = new SqlDataAdapter("select Name,Expires_on from SSL WHERE Isdeleted='False' ", strconnection);
                DataTable dtSource = new DataTable();
                da.Fill(dtSource);
                DataRow[] dr = new DataRow[dtSource.Rows.Count];
                dtSource.Rows.CopyTo(dr, 0);
                foreach (DataRow row in dr)
                {

                    row1 = new Dates()
                    {
                        Domain_name = row.ItemArray[0].ToString(),
                        Exp = (Int64)row.ItemArray[1],

                    };
                    test.Add(row1);
                }
                return test;
            }
            return null;
        }

        [SwaggerOperation(Summary = "Gets all the domains with SSL in the application.")]
        [HttpGet("GetAllSSL")]
        //[Authorize]
        public List<SSL> GetAllSSL()
        {
            //var identity = User.Identity as ClaimsIdentity;
            //if (identity != null)
            //{

                List<SSL> test = new List<SSL>();
                SSL row1 = new SSL();


                SqlDataAdapter da = new SqlDataAdapter("select * from SSL WHERE Isdeleted='False' order by Expires_on", strconnection);
                DataTable dtSource = new DataTable();
                da.Fill(dtSource);
                DataRow[] dr = new DataRow[dtSource.Rows.Count];
                dtSource.Rows.CopyTo(dr, 0);
                foreach (DataRow row in dr)
                {

                    row1 = new SSL()
                    {

                        Name = row.ItemArray[0].ToString(),
                        Issued_by = row.ItemArray[1].ToString(),
                        Issued_to = row.ItemArray[2].ToString(),
                        Issued_on = (Int64)row.ItemArray[3],
                        Expires_on = (Int64)row.ItemArray[4],
                        Certificate_type = row.ItemArray[5].ToString(),
                        Auto_renewal_enabled = (bool)row.ItemArray[6],
                        //  Isdeleted = row.ItemArray[7].ToString(),
                    };
                    test.Add(row1);
                }
                return test;
            //}
            //return null;
        }
        [SwaggerOperation(Summary = "Gets a particular SSL by domain name.")]
        [HttpGet("GetBySSLName")]
        [Authorize]
        public SSL GetBySSLName(string SSL_NAME)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {

                try
                {
                    SqlDataReader reader = null;
                    SqlConnection myConnection = new SqlConnection();
                    myConnection.ConnectionString = strconnection;

                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT * from SSL WHERE Name=' " + SSL_NAME + " ' ";
                    sqlCmd.Connection = myConnection;
                    myConnection.Open();
                    reader = sqlCmd.ExecuteReader();
                    SSL ssl = null;
                    reader.Read();

                    ssl = new SSL();

                    ssl.Name = reader.GetValue(0).ToString();
                    ssl.Issued_by = reader.GetValue(1).ToString();
                    ssl.Issued_to = reader.GetValue(2).ToString();
                    ssl.Issued_on = Convert.ToInt64(reader.GetValue(3));
                    ssl.Expires_on = Convert.ToInt64(reader.GetValue(4));
                    ssl.Certificate_type = reader.GetValue(5).ToString();
                    ssl.Auto_renewal_enabled = Convert.ToBoolean(reader.GetValue(6));
                    //  ssl.Isdeleted = reader.GetValue(7).ToString();

                    myConnection.Close();
                    if (reader == null)

                    {
                        ssl = new SSL();

                        ssl.Name = "";
                        ssl.Issued_by = "";
                        ssl.Issued_to = "";
                        ssl.Issued_on = 0;
                        ssl.Expires_on = 0;
                        ssl.Certificate_type = "";
                        ssl.Auto_renewal_enabled = false;

                    }
                    return ssl;
                }
                catch
                {
                    return null;

                }
            }
            return null;
        }
       

        [SwaggerOperation(Summary = "Add new SSL for a domain in the application.", Description = "While adding a new SSL the certificate type should be either 'STANDARD SSL' or 'STANDARD WILDCARD SSL'. ")]
        [HttpPost("AddNewSSL")]
        //[Authorize]
        public async Task<ObjectResult> NewSSL(SSL ssl)
        {
            //var identity = User.Identity as ClaimsIdentity;
            //if (identity != null)
            //{

                try
                {


                    SqlConnection myConnection = new SqlConnection();
                    myConnection.ConnectionString = strconnection;

                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;

                    sqlCmd.CommandText = "INSERT INTO SSL (Name,Issued_by,Issued_to,Issued_on,Expires_on,Certificate_type,Auto_renewal_enabled,Isdeleted) Values('" + ssl.Name + "','" + ssl.Issued_by + "','" + ssl.Issued_to + "','" + ssl.Issued_on + "','" + ssl.Expires_on + "','" + ssl.Certificate_type + "','" + ssl.Auto_renewal_enabled + "','False')";
                    myConnection.Open();
                    sqlCmd.Connection = myConnection;
                    int rowInserted = sqlCmd.ExecuteNonQuery();
                    myConnection.Close();
                    if (rowInserted != 0)
                        return new ObjectResult(new ApiResponse
                        {
                            StatusCode = 200,
                            Result = "Record Added ",
                            HasError = false,
                            Message = "A new row with Name " + ssl.Name + " is Successfully Added in the SSL table",
                            RequestTime = DateTime.UtcNow
                        });

                    else
                        return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse
                        {
                            StatusCode = 400,
                            Result = "Invalid Input",
                            HasError = false,
                            Message = "Invaild input due to violation of primary key constraint",
                            RequestTime = DateTime.UtcNow
                        });

                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                    {
                        StatusCode = 500,
                        Result = "Invalid Input",
                        Message = ex.Message,
                        HasError = false,
                        RequestTime = DateTime.UtcNow
                    });

                }
            //}
            //return null;
        }
        [SwaggerOperation(Summary = "Update issued date and expire date for a SSL.")]
        [HttpPut("UpdateDates")]
        //[Authorize]
        public async Task<ObjectResult> UpdateExpDateSSL(string NAME, int ISSUED_ON, int EXPIRES_ON)
        {
            //var identity = User.Identity as ClaimsIdentity;
            //if (identity != null)
            //{

                try
                {
                    SqlConnection myConnection = new SqlConnection();
                    myConnection.ConnectionString = strconnection;
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "UPDATE SSL  SET Issued_on='" + ISSUED_ON + "',Expires_on='" + EXPIRES_ON + "' WHERE Name='" + NAME + "'";
                    sqlCmd.Connection = myConnection;
                    myConnection.Open();
                    int rowupdated = sqlCmd.ExecuteNonQuery();
                    myConnection.Close();
                    if (rowupdated != 0)
                        return new ObjectResult(new ApiResponse
                        {
                            StatusCode = 200,
                            Result = "Record Updated ",
                            HasError = false,
                            Message = "The row from SSL table  with Name " + NAME + " is Successfully Updated",
                            RequestTime = DateTime.UtcNow
                        });
                    else
                        return StatusCode(StatusCodes.Status404NotFound, new ApiResponse
                        {
                            StatusCode = 404,
                            Result = "Not Found",
                            HasError = false,
                            Message = "The data you have entered doesn't exists in the database",
                            RequestTime = DateTime.UtcNow
                        });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                    {
                        StatusCode = 500,
                        Result = "Invalid Input",
                        Message = ex.Message,
                        HasError = false,
                        RequestTime = DateTime.UtcNow
                    });
                }
            //}
            //return null;
        }
        [SwaggerOperation(Summary = "Soft delete a SSL based on domain name.")]
        [HttpDelete("SoftDeleteSSL")]
        [Authorize]
        public async Task<ObjectResult> SoftDeleteSSL(string NAME)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {

                try
                {
                    SqlConnection myConnection = new SqlConnection();
                    myConnection.ConnectionString = strconnection;
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "UPDATE SSL  SET Isdeleted='True' WHERE Name='" + NAME + "'";
                    sqlCmd.Connection = myConnection;
                    myConnection.Open();
                    int rowupdated = sqlCmd.ExecuteNonQuery();
                    myConnection.Close();
                    if (rowupdated != 0)
                        return new ObjectResult(new ApiResponse
                        {
                            StatusCode = 200,
                            Result = "Record deleted ",
                            HasError = false,
                            Message = "The row from Domain table  with Name " + NAME + " is Successfully soft Deleted",
                            RequestTime = DateTime.UtcNow
                        });
                    else
                        return StatusCode(StatusCodes.Status404NotFound, new ApiResponse
                        {
                            StatusCode = 404,
                            Result = "Not Found",
                            HasError = false,
                            Message = "The data you have entered doesn't exists in the database",
                            RequestTime = DateTime.UtcNow
                        });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                    {
                        StatusCode = 500,
                        Result = "Invalid Input",
                        Message = ex.Message,
                        HasError = false,
                        RequestTime = DateTime.UtcNow
                    });
                }
            }
            return null;
        }
        [SwaggerOperation(Summary = "Hard delete a SSL based on domain name.")]
        [HttpDelete("HardDeleteSSL")]
       // [Authorize]
        public async Task<ObjectResult> DeleteSSL(string NAME)
        {
            //var identity = User.Identity as ClaimsIdentity;
            //if (identity != null)
            //{

                try
                {
                    SqlConnection myConnection = new SqlConnection();
                    myConnection.ConnectionString = strconnection;
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "delete  from SSL where Name='" + NAME + "' ";
                    sqlCmd.Connection = myConnection;
                    myConnection.Open();
                    int rowDeleted = sqlCmd.ExecuteNonQuery();
                    myConnection.Close();
                    if (rowDeleted != 0)
                        return new ObjectResult(new ApiResponse
                        {
                            StatusCode = 200,
                            Result = "Record deleted ",
                            HasError = false,
                            Message = ("The row from SSL table  with Name " + NAME + " is Successfully Deleted"),
                            RequestTime = DateTime.UtcNow
                        });
                    else
                        return StatusCode(StatusCodes.Status404NotFound, new ApiResponse
                        {
                            StatusCode = 404,
                            Result = "Not Found",
                            HasError = false,
                            Message = "The data you have entered doesn't exists in the database",
                            RequestTime = DateTime.UtcNow
                        });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                    {
                        StatusCode = 500,
                        Result = "Invalid Input",
                        Message = ex.Message,
                        HasError = false,
                        RequestTime = DateTime.UtcNow
                    });
                }
            //}
            //return null;
        }
    }
}
