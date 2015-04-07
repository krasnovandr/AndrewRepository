using System;
using Microsoft.AspNet.SignalR;

namespace AudioNetwork.Hubs
{
    public class TestUserModel
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
    }
    public class ConversationHub : Hub
    {
        //static List<TestUserModel> Users = new List<TestUserModel>();
        //private readonly IUserRepository _userRepository;
        //private readonly IConversationRepository _conversationRepository;

        //public ConversationHub(
        //    IUserRepository userRepository,
        //    IConversationRepository conversationRepository)
        //{
        //    _userRepository = userRepository;
        //    _conversationRepository = conversationRepository;
        //}

        public void GreetAll(int a)
        {
            a++;
            Clients.All.acceptGreet(a + "Good morning! The time is " + DateTime.Now.ToString());
        }

        public void SendedMessage()
        {

            Clients.All.newMessage();
        }

        public void ChangeConversationSong()
        {

            Clients.All.conversationSongChanged();
        }


        //public void ParamCheck(string name,int age)
        //{
        //    Clients.AllExcept(Context.ConnectionId).acceptGreet(name+ age);
        //}

        //public void Send(string name, string message)
        //{
        //    Clients.All.addMessage(name, message);
        //}

        //// Подключение нового пользователя
        //public void Connect(string userName)
        //{
        //    var id = Context.ConnectionId;


        //    if (!Users.Any(x => x.ConnectionId == id))
        //    {
        //        Users.Add(new TestUserModel { ConnectionId = id, Name = userName });

        //        // Посылаем сообщение текущему пользователю
        //        Clients.Caller.onConnected(id, userName, Users);

        //        // Посылаем сообщение всем пользователям, кроме текущего
        //        Clients.AllExcept(id).onNewUserConnected(id, userName);
        //    }
        //}

        //// Отключение пользователя
        //public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        //{
        //    var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        //    if (item != null)
        //    {
        //        Users.Remove(item);
        //        var id = Context.ConnectionId;
        //        Clients.All.onUserDisconnected(id, item.Name);
        //    }

        //    return base.OnDisconnected(stopCalled);
        //}
    }
}