using System.Collections;
using System.Text.RegularExpressions;
using API.Data;
using API.Entities;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Commons
{
    public class UsersCommon : ControllerBase
    {
        private DataContext _context;
        // private BusinessCommon _businessCommon;

        public UsersCommon(DataContext context)
        {
            _context = context;
            // this._businessCommon = new BusinessCommon(context);
        }

        public async Task<bool> UserIdExist(string id)
        {
            var result = await _context.Users.Where(x => x.Status == (int)StatusEnum.enable || x.Id.ToString() == id).AnyAsync();
            return result;
        }
        public async Task<bool> UserNameExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
        public async Task<bool> UserEmailExist(string useremail)
        {
            return await _context.Users.AnyAsync((x) => x.Email != null && x.Email.ToLower() == useremail.ToLower());
        }
        public AppUser? GetUserById(string id)
        {
            var result = _context.Users.Where(x => x.Status == (int)StatusEnum.enable || x.Id.ToString() == id).FirstOrDefault();
            return result;
        }
        public bool IsValidPassword(string password)
        {
            // Validate strong password
            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            var test = validateGuidRegex.IsMatch(password);
            return test;
        }
        public string Deletedkeyword { get; } = "deleted_";

        public ArrayList UserStatus()
        {
            ArrayList _status = new()
            {
                0,
                1
            };

            return _status;
        }

        // public TotalUsersDto GetTotalUsers(int userid = 0)
        // {
        //     var userbusiness = this._businessCommon.GetUserBusiness(userid);
        //     var employees = 0;
        //     var allUsers = 0;

        //     if (userbusiness.Count > 0)
        //     {
        //         var resultEmployees = this._context.Users.Where(x => x.Status == (int)StatusEnum.enable && x.Role != (int)RoleEnum.suadmin && x.UserProperties.All(up => userbusiness.Contains(up.Property.BusinessId) && up.IsEmployee)).ToList();
        //         employees = resultEmployees.Count;

        //         var resultAllUsers = this._context.Users.Where(x => x.Status == (int)StatusEnum.enable && x.Role != (int)RoleEnum.suadmin && x.UserProperties.All(up => userbusiness.Contains(up.Property.BusinessId))).ToList();
        //         allUsers = resultAllUsers.Count;
        //     }

        //     var userProperties = this._context.UserProperties.Where(up => up.UserId == userid && up.Status == (int)StatusEnum.enable).ToList();
        //     ArrayList _propertyList = new ArrayList();
        //     for (int i = 0; i < userProperties.Count; i++)
        //     {
        //         _propertyList.Add(userProperties[i].PropertyId);
        //     }

        //     if (_propertyList.Count > 0)
        //     {
        //         var resultEmployees2 = this._context.Users.Where(x => x.Status == (int)StatusEnum.enable && x.Role != (int)RoleEnum.suadmin && x.UserProperties.All(up => _propertyList.Contains(up.PropertyId) && up.IsEmployee)).ToList();
        //         employees = employees + resultEmployees2.Count;

        //         var resultAllUsers2 = this._context.Users.Where(x => x.Status == (int)StatusEnum.enable && x.Role != (int)RoleEnum.suadmin && x.UserProperties.All(up => _propertyList.Contains(up.PropertyId))).ToList();
        //         allUsers = allUsers + resultAllUsers2.Count;
        //     }

        //     var result = new TotalUsersDto
        //     {
        //         employees = employees,
        //         all = allUsers
        //     };

        //     return result;
        // }

        public async Task<bool> CheckGoogleAuthToken(string token = "", string urlHost = "")
        {
            try
            {
                string url = urlHost + "/tokeninfo?id_token=" + token;

                var client = new HttpClient();
                var result = await client.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}