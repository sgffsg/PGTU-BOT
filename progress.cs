using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestBOTForPGTU
{
    class Progress
    {
        public string num { get; set; }

        public List<ProgressSemestr> progressSemestr;
        public async void PrintSubject(Autorisation.AuthData authData)
        {
            string print="";
            progressSemestr = authData.progressSemestr;
            for (int i = 0; i<progressSemestr.Count; i++)
            {
                string name = progressSemestr[i].subjectName;
                print += i + 1 + ") " + name + "\n";
            }
            authData.myState = AuthStates.subjectNum;
            Interface.PrintProgress(print);
        }

        public async void PrintProgress()
        {
            int num = int.Parse(Progress.Instance().num);
            string name = progressSemestr[num-1].subjectName;
            string teacher = progressSemestr[num-1].subjectKafedra;
            float mark = progressSemestr[num - 1].totalBalls;
            DateTime date = progressSemestr[num - 1].markDate;
            Interface.PrintSubjectProgress(name, teacher, mark, date);
        }

        private static Progress instance;
        public static Progress Instance()
        {
            if (instance == null)
            {
                instance = new Progress();
            }
            return instance;
        }
        
        public class ProgressSemestr
        {
            public List<Atts> atts { get; set; }
            public float actualBall { get; set; }
            public string altSubjectName { get; set; }
            public string blockName { get; set; }
            public int dictMarkId { get; set; }
            public int dictMarkReasonId { get; set; }
            public int dictSubjectTypeControlId { get; set; }
            public int displayMode { get; set; }
            public float dopBall { get; set; }
            public float examBall { get; set; }
            public bool inMoodle { get; set; }
            public DateTime isInRitm { get; set; }
            public bool isOutOfSession { get; set; }
            public int mark { get; set; }
            public DateTime markDate { get; set; }
            public string markDescription { get; set; }
            public string markExamRecommended { get; set; }
            public string markIcon { get; set; }
            public string markReasonName { get; set; }
            public string markRecommended { get; set; }
            public int markValue { get; set; }
            public int numCourse { get; set; }
            public int numSemestr { get; set; }
            public float recalcBalls { get; set; }
            public float resultBalls { get; set; }
            public float ritmBall { get; set; }
            public float semestrControlBall { get; set; }
            public int studentId { get; set; }
            public int studyYear { get; set; }
            public string subjectKafedra { get; set; }
            public string subjectName { get; set; }
            public int subjectType { get; set; }
            public string subjectTypeControlSName { get; set; }
            public int tkId { get; set; }
            public float totalBalls { get; set; }
            public float totalBallsExam { get; set; }
            public int workPlanSubjectItemId { get; set; }
            public class Atts
            {
                public int workPlanSubjectItemId { get; set; }
                public int numAttestation { get; set; }
                public float minBall { get; set; }
                public float maxBall { get; set; }
                public float attBall { get; set; }
            }
         }
        public class UnivuzApiService : IDisposable
        {
            private Autorisation.UnivuzApiClient client;
            public UnivuzApiService()
            {
                client = new Autorisation.UnivuzApiClient();
            }

            public async Task<ProgressSemestr> GetProgress(int studentId, int semestr)
            {
                ProgressSemestr stInf = null;
                var tmp = await client.Get($"ProgressSemestr/GetProgressSemestrWithAtt/{studentId}/{semestr}", typeof(ProgressSemestr));
                if (tmp is ProgressSemestr progressSemestr)
                {
                    stInf = progressSemestr;
                }
                else
                    throw new Exception((tmp as Autorisation.ErrorMsg)?.Message);
                return stInf;
            }
            public void Dispose()
            {
                client?.Dispose();
            }
        }




    }
}
