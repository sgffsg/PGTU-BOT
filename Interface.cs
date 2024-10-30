using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace TestBOTForPGTU
{
    public enum AuthStates { start, login, pass, success, unSuccess, menu,
        back,
        subject,
        subjectNum,
        offersback
    }
    class Interface
    {
        private static string token { get; set; } = "1695560671:AAHmtvcYFumZZ9Ccs9r7p6LWDxtuiNBLRTw";

        #region Msgs
        private static string startStr { get; set; } = "Добро пожаловать! \nЯ официальный бот Volgatech в Telegram. \n \nНаш сайт:https://www.volgatech.net/ \n \nДля далнейшей работы" +  "\n*Авторизуйтесь*";
        private static string pushButtonStr { get; set; } = "Для дальнейшей работы необходимо авторизоваться.\nПерейдите к *Авторизоваться*";
        private static string autorisStr { get; set; } = "Все функции доступны только для авторизованных пользователей. \n \nЧтобы авторизоваться, введите поочередно логин и пароль вашей учетной записи. \n \nВведите логин в формате s_";
        private static string enterPass { get; set; } = "Введите пароль";
        private static string successMessage { get; set; } = ", теперь вы авторизованы. \n\n Вы можете запрашивать \nрасписание на текущую дату или на всю неделю, \nбаллы по системе <<РИТМ>> \nпо отдельным курсам или по всем курсам в целом, \nновости университета \n\nПожалуйста, используйте" + " *Меню*" + " для доступа к функциям.";
        private static string unSuccessMessage { get; set; } = "К сожалению, вы не можете авторизоваться. \n \nВведенные логин или пароль неверны. \n \nЧтобы повторить попытку, нажмите" + "\n*Повторить авторизацию*.";
        private static string menuMessage { get; set; } = "Пожалуйста, выберите пункт меню";
        private static string unCalendarMessage { get; set; } = "Сегодня занятий нет!\nВремя отдохнуть))";
        private static string ritmMessage { get; set; } = "Здесь вы можете узнать ваши баллы по систему «РИТМ»\n\nЧтобы узнать баллы по нужному курсу, перейдите в" + "\n *Выбрать курс*";
        private static string unNewsMessage { get; set; } = "К сожалению нет ничего нового =(\n";
        private static string offersMessage { get; set; } = "Будет здорово, если Вы поделитесь впечатлениями,пожеланиями и претензиями, полученными при использовании чат-бота\n\nМы рады получить объективный отзыв для дальнейшего улучшения системы =)";
        private static string selectSubjectMessage { get; set; } = "Текущий список курсов, участником которых Вы являетесь\n";
        private static string subjectMessage { get; set; } = "Чтобы узнать балл, введите цифру, соответствующую нужному курсу\n";
        private static string selectMessage { get; set; } = "\n\nЧтобы узнать балл по другому курсу, введите соответсвующую цифру";
        #endregion

        private static MessageEventArgs messageEventArgs;

        //private static AuthStates authStates { get; set; } = AuthStates.start;

        private static TelegramBotClient client;
        static void Main(string[] args)
        {
            client = new TelegramBotClient(token);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
        }

        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            messageEventArgs = e;
            var msg = e.Message;
            var authData = Autorisation.AuthInfo.VerificationUser(msg.Chat.Id);
            Console.WriteLine(msg.Chat.Id + " " + msg.Chat.FirstName + " " + msg.Text);
            switch (authData.myState)
            {
                case AuthStates.start:
                    switch (msg.Text)
                    {
                        case "/start":
                            await client.SendTextMessageAsync(msg.Chat.Id, startStr, parseMode:ParseMode.Markdown, replyMarkup: GetButtonAutorisition());
                            Console.WriteLine(authData.myState);
                            break;
                        case "Авторизоваться":
                            await client.SendTextMessageAsync(msg.Chat.Id, autorisStr, replyMarkup: RemoveButton());
                            authData.myState = AuthStates.login;
                            break;
                        default:
                            await client.SendTextMessageAsync(msg.Chat.Id, pushButtonStr, parseMode: ParseMode.Markdown, replyMarkup: GetButtonAutorisition());
                            break;
                    }
                    break;
                case AuthStates.login:
                    authData.login = msg.Text;
                    await client.SendTextMessageAsync(msg.Chat.Id, enterPass);
                    authData.myState = AuthStates.pass;
                    break;
                case AuthStates.pass:
                    authData.pass = msg.Text;
                    Autorisation.AuthInfo.Instance().GetGetToken(authData);
                    break;
                case AuthStates.subjectNum:
                    switch (msg.Text)
                    {
                        case "Вернуться в Меню":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, menuMessage, replyMarkup: GetButtonMenu());
                            authData.myState = AuthStates.menu;
                            break;
                        case "Выйти":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, autorisStr, replyMarkup: RemoveButton());
                            authData.myState = AuthStates.login;
                            break;
                        default:
                            Progress.Instance().num = msg.Text;
                            Progress.Instance().PrintProgress();
                            authData.myState = AuthStates.subjectNum;
                            //authData.myState = AuthStates.back;
                            break;
                    }
                    break;
                case AuthStates.success:
                    switch (msg.Text)
                    {
                        case "Меню":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, menuMessage, replyMarkup: GetButtonMenu());
                            authData.myState = AuthStates.menu;
                            break;
                        case "Выйти":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, autorisStr, replyMarkup: RemoveButton());
                            authData.myState = AuthStates.login;
                            break;
                    }
                    break;
                case AuthStates.back:
                    switch (msg.Text)
                    {
                        case "Вернуться в Меню":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, menuMessage, replyMarkup: GetButtonMenu());
                            authData.myState = AuthStates.menu;
                            break;
                        case "Выйти":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, autorisStr, replyMarkup: RemoveButton());
                            authData.myState = AuthStates.login;
                            break;
                    }
                    break;
                case AuthStates.offersback:
                    switch (msg.Text)
                    {
                        case "Вернуться в Меню":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, menuMessage, replyMarkup: GetButtonMenu());
                            authData.myState = AuthStates.menu;
                            break;
                        case "Выйти":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, autorisStr, replyMarkup: RemoveButton());
                            authData.myState = AuthStates.login;
                            break;
                        default:
                            await client.SendTextMessageAsync("@supp_bot_pgtu", msg.Text);
                            authData.myState = AuthStates.offersback;
                            break;
                    }
                    break;
                case AuthStates.subject:
                    switch (msg.Text)
                    {
                        case "Выбрать курс":
                            authData.myState = AuthStates.back;
                            Progress.Instance().PrintSubject(authData);   
                            break;
                        case "Вернуться в Меню":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, menuMessage, replyMarkup: GetButtonMenu());
                            authData.myState = AuthStates.menu;
                            break;
                        case "Выйти":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, autorisStr, replyMarkup: RemoveButton());
                            authData.myState = AuthStates.login;
                            break;
                    }
                    break;
                case AuthStates.unSuccess:
                    switch (msg.Text)
                    {
                        case "Повторить авторизацию":
                            await client.SendTextMessageAsync(msg.Chat.Id, autorisStr, replyMarkup: RemoveButton());
                            authData.myState = AuthStates.login;
                            break;
                    }
                    break;
                case AuthStates.menu:
                    switch (msg.Text)
                    {
                        case "Профиль":
                            Autorisation.AuthInfo.Instance().GetGetProfile(authData);

                            //сюда мутите профиль
                            break;
                        case "Расписание":
                            //сюда мутите Расписание
                            PrintCalendar();
                            authData.myState = AuthStates.back;
                            break;
                        case "РИТМ":
                            //сюда мутите РИТМ
                            authData.myState = AuthStates.subject;

                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, ritmMessage, parseMode: ParseMode.Markdown, replyMarkup: GetButtonRITM());
                            break;
                        case "Новости":
                            //сюда мутите Новости
                            PrintNews();
                            authData.myState = AuthStates.back;
                            break;
                        case "Мои предложения":
                            //сюда мутите Мои предложения
                            PrintOffers(authData);
                            break;
                        case "Выйти":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, autorisStr, replyMarkup: RemoveButton());
                            authData.myState = AuthStates.login;
                            break;
                        case "Меню":
                            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, menuMessage, replyMarkup: GetButtonMenu());
                            break;
                    }
                    break;

            }
        }
        public async static void PrintAuthSuccessMessage(Autorisation.AuthData authData)
        {
            authData.myState = AuthStates.success;
            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, authData.studentInfo.firstName + successMessage, parseMode: ParseMode.Markdown, replyMarkup: GetButtonMenuExit());
        }
        public async static void PrintAuthUnSuccessMessage()
        {
            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, unSuccessMessage, parseMode: ParseMode.Markdown, replyMarkup: GetButtonUnSuccessAuth());
        }
        public async static void PrintCalendar()
        {
            string ritm = CalendarView.PrintCalend();//authStates = AuthStates.success;
            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, unCalendarMessage + ritm, replyMarkup: GetButtonExitMenu());
        }
        public async static void PrintProfile(string name, string number, string faculty, string specialization, int course, string group, string academ, string email)
        {
                await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "*Студент*: " + name + "\n*Номер зачётной книжки*: " + number + "\n*Факультет (Институт)*: " + faculty + "\n*Профиль*: " + specialization + "\n*Курс*: " + course + "\n*Группа*: " + group + "\n*Академический статус*: " + academ + "\n*Электронная почта*: " + email , parseMode: ParseMode.Markdown, replyMarkup: GetButtonExitMenu());
        }
        public async static void PrintNews()
        {
            string news = News.NewsView.PrintNews();
            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, unNewsMessage + news, replyMarkup: GetButtonExitMenu());
        }
        public async static void PrintProgress(string subject)
        { 
            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, selectSubjectMessage + "\n" + subject + "\n" +subjectMessage, parseMode: ParseMode.Markdown, replyMarkup: GetButtonExitMenu());
        }

        public async static void PrintSubjectProgress(string subjectName, string person, float mark, DateTime date)
        {
            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, "*Наименование курса*: " + subjectName + "\n*Руководитель*: " + person + "\n*Текущий балл*: " + mark + "\n\n_Информация актуальна на_ " + date + selectMessage, parseMode: ParseMode.Markdown, replyMarkup: GetButtonExitMenu());
            //authStates = AuthStates.subjectNum;
        }

        public async static void PrintOffers(Autorisation.AuthData authData)
        {
            authData.myState = AuthStates.offersback;

            await client.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, offersMessage, replyMarkup: GetButtonExitMenu());
        }
        #region Buttons
        private static IReplyMarkup GetButtonAutorisition()
        {
            var Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Авторизоваться" } }
                };
            return new ReplyKeyboardMarkup(Keyboard, resizeKeyboard: true);
        }

        private static IReplyMarkup GetButtonMenuExit()
        {
            var Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Меню" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Выйти" } }
                };
            return new ReplyKeyboardMarkup(Keyboard, resizeKeyboard: true);
        }

        private static IReplyMarkup GetButtonExitMenu()
        {
            var Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Вернуться в Меню" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Выйти" } }
                };
            return new ReplyKeyboardMarkup(Keyboard, resizeKeyboard: true);
        }

        private static IReplyMarkup GetButtonUnSuccessAuth()
        {
            var Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Повторить авторизацию" } }
                };
            return new ReplyKeyboardMarkup(Keyboard, resizeKeyboard: true);
        }

        private static IReplyMarkup GetButtonMenu()
        {
            var Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Профиль" }, new KeyboardButton { Text = "Расписание" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "РИТМ" },  new KeyboardButton { Text = "Новости" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Мои предложения" }, new KeyboardButton { Text = "Выйти" } }
                };
            return new ReplyKeyboardMarkup(Keyboard, resizeKeyboard: true);
        }

        private static IReplyMarkup GetButtonShedule()
        {
            var Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Расписание на всю неделю" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Вернуться в Меню" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Выйти" } }
                };
            return new ReplyKeyboardMarkup(Keyboard, resizeKeyboard: true);
        }
        private static IReplyMarkup GetButtonRITM()
        {
            var Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Выбрать курс" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Вернуться в Меню" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Выйти" } }
                };
            return new ReplyKeyboardMarkup(Keyboard, resizeKeyboard: true);
        }

        private static IReplyMarkup RemoveButton()
        {
            return new ReplyKeyboardRemove { };

        }
    }
    #endregion
}
