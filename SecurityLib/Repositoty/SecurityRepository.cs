using System;
using System.Transactions;
using MyClubLib.Models;
using MyClubLib.Repository;
using System.Web.Security;
using SecurityLib.Models;
using System.Linq;

namespace SecurityLib.Repositoty
{
    public class SecurityRepository 
    {
        private readonly MyclubSecurity _db;
        private readonly EFClubRepository _repository;
        
        public SecurityRepository()
        {
            _db = new MyclubSecurity();
            _repository = new EFClubRepository();
        }
        //      private void ValidateUser(string user,string userPass, string password)
        //     {
        //       if (user == null || !BCrypt.Net.BCrypt.Verify(password, userPass) )
        //       {
        //             throw new Exception("Invalid username or password!");
        //        }
        //    }


        public User_Profile FindProfile(string email) => _db.Set<User_Profile>().SingleOrDefault(p => p.UserName == email);
        public Person Register(int? userId, string PersonName,string Password,string Gender, string address, DateTime BirthDate, string MobileNumber, string HomePhoneNumber,
                                 string Email, string Address, string Nationality)
        {
            try
            {
                // Create the membership user
                MembershipCreateStatus status;
                MembershipUser newUser = Membership.CreateUser(Email, Password, Email, null, null, true, out status);

                if (status != MembershipCreateStatus.Success)
                {
                    throw new Exception($"Failed to create user: {status}");
                }

                Person person = _repository.CreatePerson(userId,
                                                     PersonName,
                                                     Password,
                                                     Gender,
                                                     BirthDate,
                                                     MobileNumber,
                                                     HomePhoneNumber,
                                                     Email,
                                                     Address,
                                                     Nationality);


                _repository.SaveChanges();
                CreateUser(person.PersonName, true);

                return person;
            }
            catch(Exception ex)
            {
                throw new Exception( ex.ToString());
            }
        }
        public bool IsUserExist(string email , string pass)
        {
            try
            {
                var user = _repository.FindByEmail(email);
                if (user == null)
                    return false;
                if (pass != null && user.Password != pass)
                    return false;
              
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public void ChangePassword(string userName, string password)
        {
            //var User = _repository.(userName);
           // if(User == null)
           //     throw new Exception("Invalid username.");

          // .// User.Password = BCrypt.Net.BCrypt.HashPassword(password);
            _repository.SaveChanges();
        }
        public int CreateUser(string userName, bool isActive)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var profile = new User_Profile()
                    {
                        UserName = userName,
                        IsActive = isActive
                    };

                    _repository.Add(profile);
                    _repository.SaveChanges();

                    return profile.UserID;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception("error in create profile", ex);
                }
            }
        }
        public bool IsActiveUser(string Email)
        {
            using (var scope = new TransactionScope())
            {
                try
                {

                    var UserName = _repository.FindByUserName(Email);
                    var user = FindProfile(UserName.PersonName);

                    if (!user.IsActive)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception("error in create profile", ex);
                }
            }
        }
        public bool AdminPermissions(string userName)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    if (userName == null)
                        throw new Exception("Invalid username.");


                    var user = FindProfile(userName);

                    if (!user.IsAdmin)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception("error in create profile", ex);
                }
            }
        }
        public void EnableUser(string username)
        {
            try
            {

                if (username == null)
                    throw new Exception("Invalid username.");

                var user = FindProfile(username);
                if (user == null)
                    throw new Exception("User not found");

                user.IsActive = true;
                _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void DisableUser(string username)
        {
            try
            {
                var user = FindProfile(username); 
                if (user == null)
                    throw new Exception("User not found");

                user.IsActive = true;
                _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    
    
    
    
    }
}