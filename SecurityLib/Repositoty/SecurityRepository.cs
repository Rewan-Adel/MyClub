using System;
using System.Transactions;
using MyClubLib.Models;
using MyClubLib.Repository;
using System.Web.Security;
using System.Linq;
using WebMatrix.WebData;
using System.Threading.Tasks;
using System.Net;

namespace SecurityLib.Repositoty
{
    public class SecurityRepository 
    {
        private readonly MyClubEntities _db;
        private readonly EFClubRepository _repository;
        
        public SecurityRepository()
        {
           
            _db = new MyClubEntities();
            _repository = new EFClubRepository();
        }
        //      private void ValidateUser(string user,string userPass, string password)
        //     {
        //       if (user == null || !BCrypt.Net.BCrypt.Verify(password, userPass) )
        //       {
        //             throw new Exception("Invalid username or password!");
        //        }
        //    }


        public User_Profile FindProfile(string username) => _db.Set<User_Profile>().SingleOrDefault(p => p.UserName == username);
        public async Task<bool> Register(int? userId, string PersonName, string Password, string Gender, string Address, DateTime BirthDate, string MobileNumber, string HomePhoneNumber,
                          string Email, string Nationality)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    //  Person person     = await _repository.CreatePerson(userId, personName, password, gender, BirthDate, mobileNumber, homePhoneNumber, email, address, nationality);
                     // Member member     = await  _repository.CreateMember(personName, person.PersonId, userId);

                    // await CreateUser(person.PersonName, true);
                    var person = new Person
                    {
                        PersonName = PersonName,
                        Password = Password,
                        Gender = Gender,
                        BirthDate = BirthDate,
                        MobileNumber = MobileNumber,
                        HomePhoneNumber = HomePhoneNumber,
                        Email = Email,
                        Address = Address,
                        Nationality = Nationality,
                        RegistrationDate = DateTime.Now,
                        isExpected = false,
                        UserId = userId
                    };

                    _db.People.Add(person);
                    await _db.SaveChangesAsync();

                    var member = new Member
                    {
                        MemberName = PersonName,
                        PersonId = person.PersonId,
                        RegistrationDate = DateTime.Now
                    };
                    _db.Members.Add(member);
                    await _db.SaveChangesAsync();

                    // Optionally, add an audit trail
                    var audit = new AuditTrail
                    {
                        ActionTypeId = 1, // Signup action type id
                        UserId = person.UserId,
                        //  IPAddress = personDto.IPAddress,
                        TransactionTime = DateTime.Now,
                        EntityId = person.PersonId,
                        EntityRecord = "Person" // or any other identifier for your entity
                    };

                   CreateUser(person.Email, true, false);
                    //_context.AuditTrails.Add(audit);/
                    //   await _context.SaveChangesAsync();
                    scope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public int? IsUserExist(string email , string pass)
        {
            try
            {
                var user = _repository.FindByEmail(email);
                if (user == null)
                    return null;
                if (pass != null && user.Password != pass)
                    return null;
              
                return user.PersonId;
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
        public int CreateUser(string PersonName, bool isActive, bool? isAdmin)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var profile = new User_Profile
                    {
                        UserName = PersonName,
                        IsActive = isActive,
                        IsAdmin = (bool)isAdmin,
                    };

                    _db.User_Profiles.Add(profile);
                    _db.SaveChanges();

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

                    var UserName = _repository.FindByEmail(Email);
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
        public async Task<bool> SignupAsync(PersonDto personDto)
        {
            // Create a new Person instance from the provided DTO
            var person = new Person
            {
                PersonName = personDto.PersonName,
                Password = personDto.Password,
                Gender = personDto.Gender,
                BirthDate = personDto.BirthDate,
                MobileNumber = personDto.MobileNumber,
                HomePhoneNumber = personDto.HomePhoneNumber,
                Email = personDto.Email,
                Address = personDto.Address,
                Nationality = personDto.Nationality,
                RegistrationDate = DateTime.Now,
                isExpected = false // or some default value
            };

            _db.People.Add(person);

            // Save the person to generate PersonId
            await _db.SaveChangesAsync();

            // Create a new Member instance if applicable
            var member = new Member
            {
                MemberName = personDto.MemberName,
                PersonId = person.PersonId,
                RegistrationDate = DateTime.Now
            };

            _db.Members.Add(member);

            // Save the member
            await _db.SaveChangesAsync();

            // Optionally, add an audit trail
            var audit = new AuditTrail
            {
                ActionTypeId = 1, // Signup action type id
                UserId = person.UserId,
                IPAddress = personDto.IPAddress,
                TransactionTime = DateTime.Now,
                EntityId = person.PersonId,
                EntityRecord = "Person" // or any other identifier for your entity
            };

            _db.AuditTrails.Add(audit);
            await _db.SaveChangesAsync();

            return true;
        }
    public class PersonDto
    {
        public string PersonName { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string MobileNumber { get; set; }
        public string HomePhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Nationality { get; set; }
        public string MemberName { get; set; }
        public string IPAddress { get; set; }
    }

   }
}