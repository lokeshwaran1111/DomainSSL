using Domainssl.Models;
using Domainssl.Schedulers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Domainssl.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DomainController : Controller
    {
        
        String strconnection = "Data Source=152.67.29.26,1565;initial catalog=augusta_intern;User ID=lokesh;Password=WvsEnQ4Z;";
        [HttpGet("GetAllDates")]
        [Authorize]
        public List<Dates> GetAllDates()
        {
            Dates row1 = new Dates();
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            { try
                {
                    List<Dates> test = new List<Dates>();
                   


                    SqlDataAdapter da = new SqlDataAdapter("select Domain_name,Expires_on from Domain WHERE Isdeleted='False' ", strconnection);
                    DataTable dtSource = new DataTable();
                    da.Fill(dtSource);
                    DataRow[] dr = new DataRow[dtSource.Rows.Count];
                    dtSource.Rows.CopyTo(dr, 0);
                    foreach (DataRow row in dr)
                    {

                        row1 = new Dates()
                        {
                            Domain_name = row.ItemArray[0].ToString(),
                            Exp = (int)row.ItemArray[1],

                        };
                        test.Add(row1);
                    }
                    return test;
                }
                catch 
                {
                    return null;
                }
                }
            return null;
            

        }
        [SwaggerOperation(Summary = "Gets all the domains in the application.")]
        [HttpGet("GetAllDomains")]
        // [Authorize]

        public List<Domain> GetAllDomains()
        {
            Domain row1 = new Domain();
            try {
                //var identity = User.Identity as ClaimsIdentity;
                //if (identity != null)
                //{
                List<Domain> test = new List<Domain>();
               


                SqlDataAdapter da = new SqlDataAdapter("select * from Domain WHERE Isdeleted='False' order by Expires_on ", strconnection);
                DataTable dtSource = new DataTable();
                da.Fill(dtSource);
                DataRow[] dr = new DataRow[dtSource.Rows.Count];
                dtSource.Rows.CopyTo(dr, 0);
                foreach (DataRow row in dr)
                {

                    row1 = new Domain()
                    {

                        Domain_name = row.ItemArray[0].ToString(),
                        Issued_by = row.ItemArray[1].ToString(),
                        Issued_to = row.ItemArray[2].ToString(),
                        Issued_on = (Int64)row.ItemArray[3],
                        Expires_on = (Int64)row.ItemArray[4],
                        Auto_renewal_enabled = (bool)row.ItemArray[5],
                        // Isdeleted = row.ItemArray[6].ToString(),
                    };
                    test.Add(row1);
                }
                return test;
                //}
                //return null;
            }
            catch (Exception e) 
            {
                return null;
            }
            return null;
        }
       
        [SwaggerOperation(Summary = "Gets a particular domain by domain name.")]
        [HttpGet("GetByDomainName")]
       // [Authorize]

        public Domain GetByDomainName(string DOMAIN_NAME)
        {
            //var identity = User.Identity as ClaimsIdentity;
            //if (identity != null)
            //{

                try
                {
                    SqlDataReader reader = null;
                    SqlConnection myConnection = new SqlConnection();
                    myConnection.ConnectionString = strconnection;

                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT * from DOMAIN WHERE Domain_name= '" + DOMAIN_NAME + " ' ";
                    sqlCmd.Connection = myConnection;
                    myConnection.Open();
                    reader = sqlCmd.ExecuteReader();
                    Domain domain = null;
                    reader.Read();

                    domain = new Domain();

                    domain.Domain_name = reader.GetValue(0).ToString();
                    domain.Issued_by = reader.GetValue(1).ToString();
                    domain.Issued_to = reader.GetValue(2).ToString();
                    domain.Issued_on = Convert.ToInt32(reader.GetValue(3));
                    domain.Expires_on = Convert.ToInt32(reader.GetValue(4));
                    domain.Auto_renewal_enabled = Convert.ToBoolean(reader.GetValue(5));

                    myConnection.Close();

                    return domain;
                }
                catch
                {
                throw new HandledException("Domain does not exist", HttpStatusCode.BadRequest);
              
            }
            //}
            //return null;
        }
       


        [SwaggerOperation(Summary = "Add new domain into the application.", Description = "Alter your Domain name if violation of primary key error occured. ")]
        [HttpPost("AddNewDomain")]
       // [Authorize]
        public async Task<ObjectResult> AddNewDomain(Domain domain)
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
             // if(  sqlCmd.CommandText = "SELECT * from DOMAIN WHERE Domain_name= '" + domain.Domain_name + " ' and Isdeleted='true' ")
                sqlCmd.CommandText = " EXEC ADDING_DOMAIN '" + domain.Domain_name + "','" + domain.Issued_by + "','" + domain.Issued_to + "','" + domain.Issued_on + "','" + domain.Expires_on + "','" + domain.Auto_renewal_enabled + "'";
                    myConnection.Open();
                    sqlCmd.Connection = myConnection;
                    int rowInserted = sqlCmd.ExecuteNonQuery();
                    myConnection.Close();
                    if (rowInserted != 0)
                        return new ObjectResult(new ApiResponse
                        {
                            StatusCode = 200,
                            Result = "Done",
                            HasError = false,
                            Message = "A new row with Name " + domain.Domain_name + " is Successfully Added in the Domain table",
                            RequestTime = DateTime.UtcNow
                        });

                    else
                        return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse
                        {
                            StatusCode = 400,
                            Result = "Invalid Input",
                            Message = "Invaild input due to violation of primary key constraint",
                            HasError = false,
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
        [SwaggerOperation(Summary = "Update issued on and expires on values for a domain.")]
        [HttpPut("UpdateDates")]
        [Authorize]
        public async Task<ObjectResult> UpdateExpDateDomain(string DOMAIN_NAME, long ISSUED_ON, long EXPIRES_ON)
        {
            //var identity = User.Identity as ClaimsIdentity;
            //if (identity != null)
            //{
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Validation Failed");
            }

            try
            {
                    SqlConnection myConnection = new SqlConnection();
                    myConnection.ConnectionString = strconnection;
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "UPDATE DOMAIN  SET Issued_on='" + ISSUED_ON + "',Expires_on='" + EXPIRES_ON + "' WHERE Domain_name='" + DOMAIN_NAME + "'";
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
                            Message = "The data from Domain table  with Domain name " + DOMAIN_NAME + " is Successfully Updated",
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
        [SwaggerOperation(Summary = "Soft delete a domain based on domain name.")]
        [HttpDelete("SoftDeleteDomain")]
      //  [Authorize]
        public async Task<ObjectResult> SoftDeleteDomain(string DOMAIN_NAME)
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
                    sqlCmd.CommandText = "UPDATE DOMAIN  SET Isdeleted='True' WHERE Domain_name='" + DOMAIN_NAME + "'";
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
                            Message = "The row from Domain table  with Domain name " + DOMAIN_NAME + " is Successfully soft Deleted",
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
        [SwaggerOperation(Summary = "Hard delete a domain based on domain name.")]
        [HttpDelete("HardDeleteDomain")]
        [Authorize]
        public async Task<ObjectResult> DeleteDomain(string DOMAIN_NAME)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Validation failed");
            }
            
            try
                {
                    SqlConnection myConnection = new SqlConnection();
                    myConnection.ConnectionString = strconnection;
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "delete  from DOMAIN where Domain_name='" + DOMAIN_NAME + "' ";
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
                            Message = "The row from Domain table  with Domain name " + DOMAIN_NAME + " is Successfully Deleted",
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
    }
}
