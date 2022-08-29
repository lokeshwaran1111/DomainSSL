using Domainssl.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Domainssl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        String strconnection = "Data Source=152.67.29.26,1565;initial catalog=augusta_intern;User ID=lokesh;Password=WvsEnQ4Z;";
        [SwaggerOperation(Summary = "To generate token for a valid user.")]
        [HttpPost("LoginUser")]
        public async Task<Login> LoginUser(string EMAIL, string PASSWORD)
        {

            Login login = null;

            try
            {

                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = strconnection;

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "Select * from Users where Email='" + EMAIL + "'AND Password='" + PASSWORD + "' AND Isdeleted='False'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();


                reader.Read();

                login = new Login();
                login.Email = reader.GetValue(0).ToString();
                //use.Password = reader.GetValue(1).ToString();
                //  use.Isdeleted = reader.GetValue(2).ToString();

                if (login.Email == EMAIL)
                {
                    var Token = await GetTokens(login.Email);
                    //return Token;
                    login.token = Token;
                    return login;
                }
                else
                {
                    login = new Login();
                    login.token = "Token not generated due to invalid email or password";
                    login.Email = EMAIL;
                    return login;
                }



                myConnection.Close();
                async Task<string> GetTokens(string Email)
                {
                    string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
                    var issuer = "http://mysite.com";  //normally this will be your site URL    

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    //Create a List of Claims, Keep claims name short    
                    var permClaims = new List<Claim>();
                    permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    permClaims.Add(new Claim("Email", Email));


                    var JWToken = new JwtSecurityToken(issuer, //Issure    
                                    issuer,  //Audience    
                                    permClaims,
                                    expires: DateTime.Now.AddSeconds(10),
                                    signingCredentials: credentials);
                    var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                    return token;
                }
            }
            catch (Exception ex)
            {
                login = new Login();
                login.token = "Token not generated due to invalid email or password";
                login.Email = EMAIL;
                return login;
            }

        }


        //[HttpGet("gettoken")]
        //public async Task<string> GetTokens(string Email)
        //{
        //    string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
        //    var issuer = "http://mysite.com";  //normally this will be your site URL    

        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    //Create a List of Claims, Keep claims name short    
        //    var permClaims = new List<Claim>();
        //    permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        //    permClaims.Add(new Claim("Email", Email));


        //    var JWToken = new JwtSecurityToken(issuer, //Issure    
        //                    issuer,  //Audience    
        //                    permClaims,
        //                    expires: DateTime.Now.AddDays(1),
        //                    signingCredentials: credentials);
        //    var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
        //    return token;
        //}
        [SwaggerOperation(Summary = "LoginAsUser", Description = "LoginAsUser")]
        [HttpPost("USerLOgin")]
        public async Task<Login> USerLOgin(Users users)
        {
            Login login = null;
            Users user1 = null;
            try
            {



                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = strconnection;



                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "Select * from Users where Email='" + users.Email + "'AND Password='" + users.Password + "' AND Isdeleted='False'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();

                reader.Read();
                
                    user1 = new Users();
                    user1.Email = reader.GetValue(0).ToString();
                    user1.Password = reader.GetValue(1).ToString();





                    if (reader != null)
                    {
                        login = new Login();
                        var Token = await GetTokens(user1.Email);
                        login.Email = users.Email;
                        login.token = Token;
                        return login;
                    }
                    else
                    {
                        login = new Login();
                        login.token = "Token not generated due to invalid email or password";
                        login.Email = users.Email;
                        return login;
                    }
                

               myConnection.Close();
                async Task<string> GetTokens(string Email)
                {
                    string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
                    var issuer = "http://mysite.com";  //normally this will be your site URL    

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    //Create a List of Claims, Keep claims name short    
                    var permClaims = new List<Claim>();
                    permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    permClaims.Add(new Claim("Email", Email));


                    var JWToken = new JwtSecurityToken(issuer, //Issure    
                                    issuer,  //Audience    
                                    permClaims,
                                    expires: DateTime.Now.AddMinutes(1),
                                    signingCredentials: credentials);
                    var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                    return token;
                }
            }
            catch (Exception ex)
            {
                login = new Login();
                login.token = "Token not generated due to invalid email or password";
                login.Email = users.Email;
                return login;
            }
            //return StatusCode(StatusCodes.Status404NotFound, new ApiResponse
            //{
            //    StatusCode = 404,
            //    Result = "Invalid Email or Password",
            //    HasError = false,
            //    Message = "User does not exist",
            //    RequestTime = DateTime.UtcNow
            //});
        }

        [Authorize]
        [SwaggerOperation(Summary = "Gets all users uses in the application.")]
        [HttpGet("GetAllUsers")]

        public List<Users> GetAllUsers()
        {

            List<Users> test = new List<Users>();
            Users users = new Users();


            SqlDataAdapter da = new SqlDataAdapter("select * from Users WHERE Isdeleted='False' ", strconnection);
            DataTable dtSource = new DataTable();
            da.Fill(dtSource);
            DataRow[] dr = new DataRow[dtSource.Rows.Count];
            dtSource.Rows.CopyTo(dr, 0);
            foreach (DataRow row in dr)
            {

                users = new Users()
                {

                    Email = row.ItemArray[0].ToString(),
                    Password = row.ItemArray[1].ToString(),
                    // Isdeleted = row.ItemArray[2].ToString(),

                };
                test.Add(users);
            }
            return test;

        }
        
        [SwaggerOperation(Summary = "LoginAsUser", Description = "LoginAsUser")]
        [HttpPost("LoginAsUser")]
        public async Task<ObjectResult> LoginAsUser(Users users)
        {
            try
            {
                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = strconnection;
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = "Select * from Users where Email='" + users.Email + "'AND Password='" + users.Password + "' AND Isdeleted='False'";
                sqlCmd.Connection = myConnection;
                myConnection.Open();
                reader = sqlCmd.ExecuteReader();
                Users login = null;
                while (reader.Read())
                {
                    login = new Users();
                    login.Email = reader.GetValue(0).ToString();
                    login.Password = reader.GetValue(1).ToString();
                    if (reader != null)
                    {
                        return new ObjectResult(new ApiResponse
                        {
                            StatusCode = 200,
                            Result = "Login Success ",
                            HasError = false,
                            Message = "The user is successfully logged in",
                            RequestTime = DateTime.UtcNow
                        });
                    }
                    else
                        return StatusCode(StatusCodes.Status404NotFound, new ApiResponse
                        {
                            StatusCode = 404,
                            Result = "Invalid Email or Password",
                            HasError = false,
                            Message = "User does not exist",
                            RequestTime = DateTime.UtcNow
                        });
                }
                myConnection.Close();
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
            return StatusCode(StatusCodes.Status404NotFound, new ApiResponse
            {
                StatusCode = 404,
                Result = "Invalid Email or Password",
                HasError = false,
                Message = "User does not exist",
                RequestTime = DateTime.UtcNow
            });
        }

        [SwaggerOperation(Summary = "To add a new user.", Description = "Data entered at token field won't be reflected anywhere. ")]
        [HttpPost("AddNewUser")]
      // [Authorize]
        public async Task<ObjectResult> AddNewUser(Users users)
        {
            //var identity = User.Identity as ClaimsIdentity;
            //if (identity != null)
            //{
            //if (!this.ModelState.IsValid)
            //{
            //    return this.BadRequest("Validation failed");
            //}
            try
                {


                    SqlConnection myConnection = new SqlConnection();
                    myConnection.ConnectionString = strconnection;

                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "INSERT INTO Users(Email,Password,Isdeleted)  Values ('" + users.Email + "','" + users.Password + "','False')";
                    sqlCmd.Connection = myConnection;
                    myConnection.Open();
                    int rowInserted = sqlCmd.ExecuteNonQuery();
                    myConnection.Close();
                    if (rowInserted != 0)
                        return new ObjectResult(new ApiResponse
                        {
                            StatusCode = 200,
                            Result = "User Added ",
                            HasError = false,
                            Message = "A new user with Email " + users.Email + " is Successfully Added into the  table",
                            RequestTime = DateTime.UtcNow
                        });

                    else
                        return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse
                        {
                            StatusCode = 400,
                            Result = "User Already exist",
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

        [SwaggerOperation(Summary = "Soft delete a user by email.")]
        [HttpDelete("SoftDeleteUser")]
        [Authorize]
        public async Task<ObjectResult> SoftDeleteUser(string EMAIL)
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
                    sqlCmd.CommandText = "UPDATE Users  SET Isdeleted='True' WHERE Email='" + EMAIL + "'";
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
                            Message = "The row from Users table  with Email " + EMAIL + " is Successfully soft Deleted",
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
        [SwaggerOperation(Summary = "Soft delete a user by email.")]
        [Authorize]
        [HttpDelete("HardDeleteUser")]
        public async Task<ObjectResult> DeleteUser(string EMAIL)
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
                    sqlCmd.CommandText = "delete  from Users where Email='" + EMAIL + "' ";
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
                            Message = "The row from Users table  with Email " + EMAIL + " is Successfully Deleted",
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

    }
}
