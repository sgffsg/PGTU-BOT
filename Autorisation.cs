using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestBOTForPGTU
{
    class Autorisation
    {
        public class AuthData
        {
            public long myChatId;
            public AuthStates myState;
            public string login { get; set; }
            public string pass { get; set; }
            public StudentProfile studentProfile;
            public StudentInfo studentInfo;
            public AuthToken token;
            public List<Progress.ProgressSemestr> progressSemestr;
            public List<CalendarView.Calendar> calen;
        }
        public class AuthInfo
        {
            public List<AuthData> authDatas { get; set; }
            private static AuthInfo instance;
            public static AuthInfo Instance()
            {
                if (instance == null)
                {
                    instance = new AuthInfo();
                    instance.authDatas = new List<AuthData>();
                }
                return instance;
            }
            public static AuthData VerificationUser(long chatID)
            {
                var authDatas = Instance().authDatas;
                var tmp = authDatas.Find(x => x.myChatId == chatID);
                if (tmp != null)
                {
                    return tmp;
                }
                else
                {
                    Console.WriteLine("New chatId " + chatID);
                    tmp = new AuthData();
                    tmp.myChatId = chatID;
                    tmp.myState = AuthStates.start;
                    authDatas.Add(tmp);
                    return tmp;
                }
            }
            public async void GetGetProgress(int studentId, int semestr)
            {
                var service = new UnivuzApiService();
                
            }
            public async void GetGetToken(AuthData authData)
            {
                //Надо раскоментировать будет try catch когда проект закончим
                try
                {
                    var service = new UnivuzApiService();
                    authData.token = await service.GetToken(authData);
                    authData.studentProfile = await service.GetProfiles(authData);
                    authData.studentInfo = await service.GetStudentInfo(authData);
                    authData.progressSemestr = await service.GetProgress(authData.studentProfile.studentId, authData.studentInfo.semestrNum);
                    //authData.calen = await service.GetCalendar(authData.studentProfile.studentId); //расписания пока нет
                    Interface.PrintAuthSuccessMessage(authData);

                }
                catch
                {
                    Interface.PrintAuthUnSuccessMessage();
                }
    }
            public async void GetGetProfile(AuthData authData)
            {
                string StudentNumber = authData.studentInfo.studentNumber;
                string name = authData.studentInfo.firstName + " " + authData.studentInfo.lastName + " " + authData.studentInfo.middleName;
                string faculty = authData.studentInfo.facultyName;
                string specialization = authData.studentInfo.specializationName;
                int course = authData.studentInfo.courseNum;
                string group = authData.studentInfo.groupName;
                string academ = authData.studentInfo.academicStateName;
                string email = "";
                authData.myState = AuthStates.back;
                if (authData.studentInfo.emails.Count != 0)
                {
                    email = authData.studentInfo.emails[0].emailAddr;
                }
                Interface.PrintProfile(name, StudentNumber, faculty, specialization, course, group, academ, email);
            }
        }
        public class ErrorMsg
        {
            public bool Status { get; set; }
            public string Message { get; set; }
        }


        public class AuthToken
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public int ExpiresIn { get; set; }
            public string Login { get; set; }
            public DateTime ExpiresDateTime { get; set; }
        }

        public class StudentProfile
        {
            public int stateLevel { get; set; }
            public int studentId { get; set; }
            public string group { get; set; }
            public int personId { get; set; }
        }
        
        public class StudentInfo
        {
            public string academicStateName { get; set; }
            public string academicStateSName { get; set; }
            public DateTime birthday { get; set; }
            public int courseNum { get; set; }
            public int dictQualificationId { get; set; }
            public int dictStudentAcademicStateId { get; set; }
            public string facultyName { get; set; }
            public string facultySName { get; set; }
            public string filialName { get; set; }
            public string filialSName { get; set; }
            public string firstName { get; set; }
            public string groupName { get; set; }
            public string kafedraName { get; set; }
            public string kafedraSName { get; set; }
            public string lastName { get; set; }
            public string lkPhoto { get; set; }
            public string middleName { get; set; }
            public string qualificationName { get; set; }
            public string qualificationSName { get; set; }
            public int semestrNum { get; set; }
            public int specializationId { get; set; }
            public string specializationName { get; set; }
            public string specializationShifr { get; set; }
            public string specializationSName { get; set; }
            public int standardId { get; set; }
            public string standardName { get; set; }
            public string standardSName { get; set; }
            public string studentNumber { get; set; }
            public string studyFormName { get; set; }
            public string studyFormSName { get; set; }
            public string studyFormTypeName { get; set; }
            public string studyFormTypeSName { get; set; }
            public string studyLength { get; set; }
            public int studyDurationYears { get; set; }
            public int studyDurationMonth { get; set; }
            public List<Email> emails { get; set; }
            public List<PhoneInf> phoneList { get; set; }
            public List<AddrList> addressList { get; set; }
            public class Email
            {
                public string emailAddr { get; set; }
                public int emailId { get; set; }
                public int emailType { get; set; }
            }
            public class PhoneInf
            {
                public string comment { get; set; }
                public int phoneId { get; set; }
                public string phoneNumber { get; set; }
                public int phoneType { get; set; }
            }
            
            public class AddrList
            {
                public int addressId { get; set; }
                public string building { get; set; }
                public string city { get; set; }
                public string comment { get; set; }
                public string country { get; set; }
                public string flat { get; set; }
                public string fullAddress { get; set; }
                public string house { get; set; }
                public bool isRegistered { get; set; }
                public string locality { get; set; }
                public string localityType { get; set; }
                public string state { get; set; }
                public string stateType { get; set; }
                public string street { get; set; }
                public string streetType { get; set; }
                public string zipCode { get; set; }
            }
        }

        public class UnivuzApiClient : IDisposable
        {
            private HttpClient client;
            private string authToken;

            private void InitClass()
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                client.BaseAddress = new Uri("https://api.volgatech.net/api/");
            }
            public UnivuzApiClient(AuthToken token)
            {
                InitClass();
                InitAuth(token);
            }

            public UnivuzApiClient()
            {
                InitClass();
            }

            public void InitAuth(AuthToken token)
            {
                if (token != null)
                {
                    authToken = token != null && token.ExpiresDateTime < DateTime.Now ? token.AccessToken : null;
                    if (!string.IsNullOrEmpty(authToken))
                    {
                        if (client.DefaultRequestHeaders.Contains("Authorization"))
                            client.DefaultRequestHeaders.Remove("Authorization");
                        client?.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
                    }
                }
            }

            /// <summary>
            /// POST запрос к API
            /// </summary>
            /// <param name="path">Путь метода из API (без BaseUrl)</param>
            /// <param name="requestContent">Объект для тела запроса (будет преобразован в JSON автоматически)</param>
            /// <param name="returnType">Тип возвращаемого из API объекта (используется для десериализации из Json)</param>
            /// <returns>Возвращает объект типа returnType или ErrorMsg в случае ошибки от API</returns>
            public async Task<object> Post(string path, object requestContent, Type returnType)
            {
                var json = JsonConvert.SerializeObject(requestContent);
                var response = await client.PostAsync(path, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                string responseBody = await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode
                    ? JsonConvert.DeserializeObject(responseBody, returnType)
                    : JsonConvert.DeserializeObject<ErrorMsg>(responseBody);

            }

            /// <summary>
            /// GET запрос к API
            /// </summary>
            /// <param name="path">Путь метода из API (без BaseUrl)</param>
            /// <param name="returnType">Тип возвращаемого из API объекта (используется для десериализации из Json)</param>
            /// <returns>Возвращает объект типа returnType или ErrorMsg в случае ошибки от API</returns>
            public async Task<object> Get(string path, Type returnType)
            {
                var response = await client.GetAsync(path).ConfigureAwait(false);
                string responseBody = await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode
                    ? JsonConvert.DeserializeObject(responseBody, returnType)
                    : JsonConvert.DeserializeObject<ErrorMsg>(responseBody);
            }

            public void Dispose()
            {
                client?.Dispose();
            }
        }




        public class UnivuzApiService : IDisposable
        {
            private UnivuzApiClient client;
            public AuthToken token;
            public StudentProfile profile;
            //public LoginCodes loginCode;
            private void _checkTokenAlive(AuthData authData)
            {
                //если токена нет - получим
                if (token == null || token.ExpiresDateTime <= DateTime.Now.AddMinutes(-1))
                    RefreshToken(authData);
            }
            public UnivuzApiService()
            {
                client = new UnivuzApiClient();
            }
            public UnivuzApiService(AuthToken token)
            {
                this.token = token;
                client = new UnivuzApiClient(token);
            }

            #region Auth

            /// <summary>
            /// Принудительное получение токена авторизации (вызов не требуется, авторизация происходит при вызове любого метода)
            /// </summary>
            /// <returns></returns>
            public async Task<AuthToken> GetToken(AuthData authData)
            {
                var tmp = await client.Post("Auth/GetToken",
                    new { password = authData.pass, login = authData.login }, typeof(AuthToken));
                if (tmp is AuthToken authToken)
                {
                    client.InitAuth(authToken);
                    token = authToken;
                }
                else
                    throw new Exception((tmp as ErrorMsg)?.Message);

                return token;
            }
            /// <summary>
            /// Принудительное обновление токена (вызов не требуется, автоматически обновляется при вызове любого метода)
            /// </summary>
            /// <returns></returns>
            public async Task<AuthToken> RefreshToken(AuthData authData)
            {
                //Если токен есть - обновим
                if (token != null)
                {
                    var tmp = await client.Post("Auth/RefreshToken",
                        new { refreshToken = token.RefreshToken }, typeof(AuthToken));
                    if (tmp is AuthToken authToken)
                    {
                        token = authToken;
                        client.InitAuth(token);
                    }
                    else
                        throw new Exception((tmp as ErrorMsg)?.Message);

                    return token;
                }
                //Если нет токена - получим новый
                return await GetToken(authData);
            }

            public async Task<StudentProfile> GetProfiles(AuthData authData)
            {
                _checkTokenAlive(authData);
                var tmp = await client.Get("Student/GetProfiles", typeof(List<StudentProfile>));
                if (tmp is List<StudentProfile> studentProfile)
                {
                    profile = studentProfile? [0];
                }
                else
                    throw new Exception((tmp as ErrorMsg)?.Message);
                return profile;
            }
            public async Task<StudentInfo> GetStudentInfo(AuthData authData)
            {
                _checkTokenAlive(authData);
                StudentInfo stInf = null;
                var tmp = await client.Get($"Student/GetInfo/{authData.studentProfile.studentId}", typeof(StudentInfo));
                if (tmp is StudentInfo studentInfo)
                {
                    stInf = studentInfo;
                }
                else
                    throw new Exception((tmp as ErrorMsg)?.Message);
                return stInf;
            }

            public async Task<List<Progress.ProgressSemestr>> GetProgress(int studentId, int semestr)
            {
                List<Progress.ProgressSemestr> stInf = null;
                var tmp = await client.Get($"ProgressSemestr/GetProgressSemestrWithAtt/{studentId}/{semestr}", typeof(List<Progress.ProgressSemestr>));
                if (tmp is List<Progress.ProgressSemestr> progressSemestr)
                {
                    stInf = progressSemestr;
                }
                else
                    throw new Exception((tmp as Autorisation.ErrorMsg)?.Message);
                return stInf;
            }

            #endregion

            public void Dispose()
            {
                client?.Dispose();
            }

            public async Task<List<CalendarView.Calendar>> GetCalendar(int studentId)
            {
                List<CalendarView.Calendar> stInf = null;
                //new DateTime(2008, 5, 1, 8, 30, 52);
                var dataStart = new DateTime(2021, 5, 1, 8, 0, 0);
                var dataEnd = new DateTime(2021, 5, 2, 8, 0, 0);
                var tmp = await client.Get($"Calendar/{studentId}/{dataStart}/{dataEnd}", typeof(List<CalendarView.Calendar>));
                if (tmp is List<CalendarView.Calendar> calen)
                {
                    stInf = calen;
                }
                else
                    throw new Exception((tmp as Autorisation.ErrorMsg)?.Message);
                return stInf;
            }
        }
    }
}
