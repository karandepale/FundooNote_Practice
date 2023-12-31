﻿using CommonLayer.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepoLayer.Context;
using RepoLayer.Entity;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq; 
using System.Security.Claims;
using System.Text;

namespace RepoLayer.Services
{
    public class UserRepo : IUserRepo
    {
        private readonly FundooContext fundooContext;
        private readonly IConfiguration configuration;
        public UserRepo(FundooContext fundooContext, IConfiguration configuration)
        {
            this.fundooContext = fundooContext;
            this.configuration = configuration;
        }


        // USER REGISTRATION METHOD IMPLEMENTATION:-
        public UserEntity UserRegistration(UserRegistrationModel model)
        {
            try
            {
                UserEntity userEntity = new UserEntity();
                userEntity.FirstName = model.FirstName;
                userEntity.LastName = model.LastName;
                userEntity.DateOfBirth = model.DateOfBirth;
                userEntity.Email = model.Email;
                userEntity.Password = model.Password;

                fundooContext.Users.Add(userEntity);
                fundooContext.SaveChanges();

                if (userEntity != null)
                {
                    return userEntity;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }



        // USER LOGIN METHOD IMPLEMENTATION:-
        public UserLoginResult UserLogin(UserLoginModel model)
        {
            try
            {
                var userEntity = fundooContext.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (userEntity != null)
                {
                    return new UserLoginResult
                    {
                        UserEntity = userEntity,
                        JwtToken = GenerateJwtToken(userEntity.Email, userEntity.UserID)
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

     





        // GET USER LIST METHOD IMPLEMENTATION:-
        public List<UserEntity> GetAllUser()
        {
            try
            {
                var result = fundooContext.Users.ToList();
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }



        //GET USER BY THIER USER-ID:-
        public UserEntity GetUserByID(long UserID)
        {
            try
            {
                var result = fundooContext.Users.FirstOrDefault
                    (data => data.UserID == UserID);

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        // JWT IMPLEMENTATION :-
        public string GenerateJwtToken(string Email, long UserId)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", UserId.ToString()),
                new Claim("Email", Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(configuration["JwtSettings:Issuer"], configuration["JwtSettings:Audience"], claims, DateTime.Now, DateTime.Now.AddHours(1), creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

       




        // FORGOT PASSWORD IMPLEMENTATION USINF MSMQ:-
        public string ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                var result = fundooContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (result != null)
                {
                    var Token = GenerateJwtToken(result.Email, result.UserID);
                    MSMQ msmq = new MSMQ();
                    msmq.SendData2Queue(Token);

                    return Token;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }



        //RESET PASSWORD IMPLEMENTATION:-
        public bool ResetPassword(string email, string newPass, string confirmPass)
        {
            try
            {
                if (newPass == confirmPass)
                {
                    var isEmailPresent = fundooContext.Users.FirstOrDefault(user => user.Email == email);
                    isEmailPresent.Password = confirmPass;
                    fundooContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


    }
}