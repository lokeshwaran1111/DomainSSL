using Domainssl.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Domainssl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        String strconnection = "Data Source=152.67.29.26,1565;initial catalog=augusta_intern;User ID=lokesh;Password=WvsEnQ4Z;";
        [SwaggerOperation(Summary = "To add a new user.", Description = "This controller is used for testing purpose. ")]
        [HttpPost("AddNewUser")]
        public async Task<ObjectResult> AddNewTestUser(Test test)
        {
            try
            {


                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = strconnection;

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "INSERT INTO TESTING(NAME,AGE,DATE)  Values ('" + test.NAME + "','" + test.AGE + "','" + test.DATE + "')";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                int rowInserted = sqlCmd.ExecuteNonQuery();
                myConnection.Close();
                if (rowInserted != 0)
                    return new ObjectResult(new ApiResponse
                    {
                        StatusCode = 200,
                        Result = "Record Added ",
                        HasError = false,
                        Message = "A new user with name " + test.NAME + " is Successfully Added into the  table",
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
        }
        [HttpPut("UpdateDates")]
        public async Task<ObjectResult> UpdateDates(string NAME, int AGE, int DATE)
        {
            try
            {
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = strconnection;
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "UPDATE TESTING  SET AGE=" + AGE + ",DATE=" + DATE + " WHERE NAME=" + NAME + "";
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
                        Message = "The data from Domain table  with Domain name " + NAME + " is Successfully Updated",
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
