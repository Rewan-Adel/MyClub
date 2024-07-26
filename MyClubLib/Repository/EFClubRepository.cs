using MyClubLib.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Transactions;

namespace MyClubLib.Repository
{
    //All methods deals with db (add, get, edit, delete)
    public class EFClubRepository
    {
        private MyClubEntities _db;
        private readonly Utilities utilities;

        public EFClubRepository()
        {
            _db = new MyClubEntities();
            utilities = new Utilities();
        }
        public void Add<T>(T entity) where T : class => _db.Set<T>().Add(entity);
        public void SaveChanges()
        {
            try
            {
                _db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                throw new Exception(exceptionMessage, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public List<T> GetAll<T>() where T : class => _db.Set<T>().ToList();
        public T Find<T>(long id) where T : class => _db.Set<T>().Find(id);
        public void Delete<T>(T entity) where T : class => _db.Set<T>().Remove(entity);
        public void DeleteAll<T>() where T : class {
            var entities = _db.Set<T>().ToList();
            _db.Set<T>().RemoveRange(entities);
            _db.SaveChanges();
        }
        public void Edit<T>(T entity) where T : class => _db.Set<T>().AddOrUpdate(entity);
        
        public Person FindByEmail(string email)=> _db.Set<Person>().SingleOrDefault(p => p.Email == email);
        public Person FindByUserName(string userName) => _db.Set<Person>().SingleOrDefault(p => p.PersonName == userName);
       
        public void CreateAudit(ActionType actionType, Action action, int? userId, MasterEntity entity, string entityRecord)
        {
            try
            {

                var Audit = new AuditTrail
                {
                    ActionTypeId = (int)actionType,
                    ActionId = (int)action,
                    UserId = userId,
                    EntityId = (int)entity,
                    EntityRecord = entityRecord,
                    TransactionTime = DateTime.Now,
                    IPAddress = utilities.GetIpAddress()
                };

                Add(Audit);
                SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
        public Person CreatePerson(int? userId, string personName, string password, string gender, DateTime BirthDate, string mobileNumber, string homePhoneNumber,
                             string email, string address, string nationality)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var newPerson = new Person()
                    {
                        PersonName = personName,
                        Password = password,
                        Gender = gender,
                        BirthDate = BirthDate,
                        MobileNumber = mobileNumber,
                        HomePhoneNumber = homePhoneNumber,
                        Email = email,
                        Address = address,
                        Nationality = nationality,
                        RegistrationDate = DateTime.Now,
                        UserId = userId,
                        isExpected = null,
                        MemberOfferId = null
                    };

                    Add(newPerson);
                    SaveChanges();
                  
                    scope.Complete();
                    return newPerson;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public void UpdatePersonMemberOfferId(Person updatedPerson)
        {
            using (var scope = new TransactionScope())
            {
                // Retrieve the existing entity
                var existingPerson = Find<Person>(updatedPerson.PersonId);

                if (existingPerson == null)
                {
                    throw new Exception("Person not found");
                }

                existingPerson.MemberOfferId = updatedPerson.MemberOfferId; // If applicable
                SaveChanges();
            }
        }


        public Member CreateMember(string memberName, int personId, int? userId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var Member = new Member
                    {
                        MemberName = memberName,
                        PersonId = personId,
                        UserId = userId,
                        RegistrationDate = DateTime.Now,
                        LastModifiedDate = DateTime.Now
                    };
                    string entityRecord = "";
                    if (userId != null)
                    {
                          entityRecord = userId != null
                          ? $"{Find<Person>((int)userId).PersonName} added new member {memberName} to the system."
                          : $"{memberName} added to the system.";
                    }
                
                    Add(Member);
                   // CreateAudit(ActionType.Add, Action.Create_Member, Member.UserId, MasterEntity.Member, entityRecord);
                    SaveChanges();

                    scope.Complete();

                    return Member;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception("error in create member", ex);
                }
            }

        }
        public MemberOffer CreateMemberOffer(int MemberId, string note, int? OfferPriceId, decimal? PaymentAmount, 
                                     offerStatus CurrentStatusId, int? CreatedById, decimal? MemberPrice, decimal? DiscountPercent, decimal? DiscountValue,
                                      int? DiscountById,int? TrainerId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var offer = new MemberOffer()
                    {
                        MemberId = MemberId,
                        OfferPriceId = OfferPriceId,
                        PaymentAmount = PaymentAmount,
                        CurrentStatusId = (int)CurrentStatusId,
                        CreatedById = CreatedById,
                        MemberPrice = MemberPrice,
                        DiscountPercent = DiscountPercent,
                        DiscountValue = DiscountValue,
                        CreationDate = DateTime.Now,
                        TrainerId = TrainerId,
                        Note = note,
                        PaymentDate = null,
                        DiscountById = null,
                        EndDate = null,
                     
                    };

                    Add(offer);
                    SaveChanges();
                    scope.Complete();

                    return offer;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.ToString());
                }


            }
        }
        public void DeleteMember(int memberId)

        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var member = Find<Member>(memberId);

                    if (member != null)
                    {
                        string entityRecord = $"Deleting {member.MemberName} successfully";
                        Delete(member);
                        CreateAudit(ActionType.Delete, Action.Delete_Member, member.UserId, MasterEntity.Member, entityRecord);
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void EditMember(int memberId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var member = Find<Member>(memberId);
                    string entityRecord = $"updating {member.MemberName} successfully";

                    if (member != null)
                    {
                        Edit(member);
                        SaveChanges();
                    }
                    CreateAudit(ActionType.Edit, Action.Edit_Member, member.UserId, MasterEntity.Member, entityRecord);

                    scope.Complete();

                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void DeletePesron(int personId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var person = Find<Person>(personId);

                    if (person != null)
                    {
                        string entityRecord = $"Deleting {person.PersonName} successfully";
                        Delete(person);
                        CreateAudit(ActionType.Delete, Action.Delete_Member, person.PersonId, MasterEntity.Member, entityRecord);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void EditPerson(int personId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var person = Find<Person>(personId);

                    if (person != null)
                    {

                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void CreateService(int? PersonId, string ServiceName,  int ServicePrice,bool IsActive)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var newService = new service()
                    {
                        ServiceName = ServiceName,
                        ServicePrice = ServicePrice,
                        CreationDate = DateTime.Now,
                        LastModifiedDate = DateTime.Now,
                        IsActive = IsActive,
                        IsDeleted = false,
                    };
                 //   var user = Find<Person>(PersonId);
                   // if (user == null)
                     //   throw new Exception("User not found");

                    //string entityRecord = $"{user.PersonName} created {ServiceName}";
                    Add(newService);
                    SaveChanges();
                   // CreateAudit(ActionType.Add, Action.Create_Service, PersonId, MasterEntity.Service, entityRecord);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void DeleteService(int serviceId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var service = Find<service>(serviceId);

                    if (service != null)
                    {
                        service.IsDeleted = true;
                        service.IsActive = false;

                        SaveChanges();
                        string entityRecord = $"Deleting {service.ServiceName} successfully";
                        CreateAudit(ActionType.Delete, Action.Delete_Service, service.ServiceId, MasterEntity.Service, entityRecord);
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void EnableService(int serviceId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var service = Find<service>(serviceId);

                    if (service != null)
                    {
                        service.IsActive = true;
                        Edit(service);
                        SaveChanges();

                        string entityRecord = $"{service.ServiceName} is enabled.";
                        CreateAudit(ActionType.Edit, Action.Enable_Service, service.ServiceId, MasterEntity.Service, entityRecord);
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void DisableService(int serviceId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var service = Find<service>(serviceId);

                    if (service != null)
                    {
                        service.IsActive = false;
                        Edit(service);
                        SaveChanges();
                        string entityRecord = $"{service.ServiceName} is disabled.";
                        CreateAudit(ActionType.Edit, Action.Disable_Service, service.ServiceId, MasterEntity.Service, entityRecord);
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void EditServiceName(int serviceId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var service = Find<service>(serviceId);
                    string oldServiceName = null;

                    if (service != null)
                    {
                        oldServiceName = service.ServiceName;
                        service.ServiceName = service.ServiceName;
                        SaveChanges();
                    }
                    string entityRecord = $"changing {oldServiceName} to {service.ServiceName}";
                    CreateAudit(ActionType.Edit, Action.Edit_ServiceName, service.ServiceId, MasterEntity.Service, entityRecord);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void EditServicePrice(int serviceId, int servicePrice)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var service = Find<service>(serviceId);

                    if (service != null)
                    {
                        service.ServicePrice = servicePrice;
                        SaveChanges();
                    }
                    string entityRecord = $"updating {service.ServiceName} price.";
                    CreateAudit(ActionType.Edit, Action.Edit_ServicePrice, service.ServiceId, MasterEntity.Service, entityRecord);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public List<service> GetAllService()
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var services = GetAll<service>();
                    scope.Complete();
                    return services;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public service GetService(int serviceId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var service = Find<service>(serviceId);
                    scope.Complete();
                    return service;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.Message);
                }
            }
        }
        public void CreateOfferPrice(decimal price, DateTime? StartDate, DateTime? EndDate, int? OfferDurationId,
            bool? IsActive, int? MaxFreezingDays)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var newOfferPrice = new OfferPrice()
                    {
                     Price = price,
                     StartDate = StartDate,
                     EndDate = EndDate,
                     OfferDurationId = OfferDurationId,
                     IsActive = IsActive,
                     MaxFreezingDays = MaxFreezingDays,
                     CreationDate   = DateTime.Now
                    };

        
                    Add(newOfferPrice);
                    SaveChanges();
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.ToString());
                }


            }
        }
        public void CreateOfferDuration(string OfferDurationName, string Description, bool? IsVisible, int? Days)

        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var newOfferPrice = new OfferDuration()
                    {
                        OfferDurationName = OfferDurationName,
                        Description = Description,
                        IsVisible = IsVisible,
                        Days = Days
                    };


                    Add(newOfferPrice);
                    SaveChanges();
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.ToString());
                }



    }
        }
        public void CreateOfferDetail(int OfferPriceId, int serviceID,  int serviceLimitNum )

        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var newOfferDetail = new OfferDetail()
                    {
                       OfferPriceId = OfferPriceId,
                       ServiceId = serviceID,
                       ServiceLimitNumber = serviceLimitNum
                    };


                    Add(newOfferDetail);
                    SaveChanges();
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.ToString());
                }



            }
        }
        public void CreateAttendance(int userId, int MemberOfferId, int ServiceId)

        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var memberAttendance = new MemberAttendance()
                    {
                        MemberOfferId = MemberOfferId,
                        ServiceId = ServiceId,
                        AttendanceDate = DateTime.Now
                    }; 
 
                    Add(memberAttendance);
                    SaveChanges();
                    var user = Find<Person>(userId);
                   // string entityRecord = $"{user.PersonName} confirm Attendance.";
                   // CreateAudit(ActionType.Add, Action.Attendance_Confirmation, userId, MasterEntity.MemberOffer, entityRecord);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception(ex.ToString());
                }



            }
        }


    }
}