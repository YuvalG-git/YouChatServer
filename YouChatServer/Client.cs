using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using YouChatServer.Encryption;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Image = System.Drawing.Image;
using System.Threading;
using System.Net.Mail;
using YouChatServer.UserDetails;
using Newtonsoft.Json;
using YouChatServer.ChatHandler;

namespace YouChatServer
{
    /// <summary>
    /// The Client class is responsible for the communication with the clients
    /// </summary>
    class Client
    {

        /// <summary>
        /// This Stores a list of all clients connecting to the server:
        /// The list is static so all the clients will be able to obtain the list of current connected client
        /// </summary>
        public static Hashtable AllClients = new Hashtable();
        //private Dictionary<string, int> _clientFailedAttempts = new Dictionary<string, int>();
        private Dictionary<IPAddress, ClientAttemptsState> _clientFailedAttempts = new Dictionary<IPAddress, ClientAttemptsState>();

        public static List<Client> clientsList = new List<Client>();

        /// <summary>
        /// Represents the maximum number of clients that were connected to the game
        /// </summary>
        public static int connectedClients = 0;

        /// <summary>
        /// A queue that contains the Id's of all the clients who left the game
        /// That way when a new player joins, he will recieve a used id
        /// </summary>
        public static Queue<int> disconnectedClients = new Queue<int>();

        // information about the client

        /// <summary>
        /// Object which represents the client's TCP client 
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// Represents the IP address of the client
        /// </summary>
        private string _clientIP;

        private IPAddress _clientAddress;

        /// <summary>
        /// Represents the client username
        /// </summary>
        private string _ClientNick;


        /// <summary>
        /// Represents the overall ID of the player
        /// </summary>
        private int PlayerNum;

        /// <summary>
        /// Represents the player's color
        /// </summary>
        private string _ClientColor;

        /// <summary>
        /// Shows if the player is the one who chooses the board's size
        /// If he is, it will change to true
        /// </summary>
        private Boolean hasChosenSize = false;

        /// <summary>
        /// Represents if the player has chosen a color for the game yet
        /// If he chose, it will change to true        
        /// </summary>
        private Boolean hasChosenColor = false;

        /// <summary>
        /// This boolean variable is true after the client disconnects
        /// It was made due to an error...
        /// </summary>
        private Boolean hasLeft = false;

        /// <summary>
        /// Byte array which represents the data received from the client
        /// It it used for both sending and reciving data
        /// </summary>
        private byte[] data;

        static Random Random = new Random();

        /// <summary>
        /// Request/Response kinds' which are used in sending and recieving messages
        /// </summary>
        const int EncryptionClientPublicKeySender = 39;
        const int EncryptionServerPublicKeyReciever = 40;
        const int EncryptionSymmetricKeyReciever = 41;
        const int PasswordRenewalMessageRequest = 42;
        const int PasswordRenewalMessageResponse = 43;
        const int InitialProfileSettingsCheckRequest = 44;
        const int InitialProfileSettingsCheckResponse = 45;
        const int FriendRequestSender = 48;
        const int FriendRequestReceiver = 49;
        const int FriendRequestResponseSender = 50;
        const int FriendRequestResponseReceiver = 51;
        const int FriendsProfileDetailsRequest = 52;
        const int FriendsProfileDetailsResponse = 53;
        const int PasswordUpdateRequest = 54;
        const int PasswordUpdateResponse = 55;
        const int UserConnectionCheckRequest = 56;
        const int UserConnectionCheckResponse = 57;
        const int PastFriendRequestsRequest = 58;
        const int PastFriendRequestsResponse = 59;
        public const int BlockBeginning = 60;
        public const int BlockEnding = 61;
        const int VideoCallRequest = 62;
        const int VideoCallResponse = 63;
        const int VideoCallResponseSender = 64;
        const int VideoCallResponseReciever = 65;
        const int GroupCreatorRequest = 66;
        const int GroupCreatorResponse = 67;
        const int UserDetailsRequest = 46;
        const int UserDetailsResponse = 47;
        const int registerRequest = 1;
        const int registerResponse = 2;
        const int loginRequest = 3;
        const int loginResponse = 4;
        const int colorRequest = 5;
        const int colorResponse = 6;
        const int playersNumRequest = 7;
        const int playersNumResponse = 8;
        const int boardSizeSender = 9;
        const int boardSizeRequest = 10;
        const int boardSizeResponse = 11;
        const int oppenentDetailsRequest = 12;
        const int oppenentDetailsResponse = 13;
        const int coordinatesSender = 14;
        const int coordinatesReciever = 15;
        const int skipTurnSender = 16;
        const int skipTurnReciever = 17;
        const int endGameRequest = 18;
        const int endGameResponse = 19;
        const int disconnectRequest = 20;
        const int loadingModeRequest = 21;
        const int lastGameRequest = 22;
        const int lastGameResponse = 23;
        const int anotherGameRequest = 24;
        const int anotherGameResponse = 25;
        const int leaveGameRequest = 26;
        const int sendMessageRequest = 28;
        const int sendMessageResponse = 29;
        const int ContactInformationRequest = 33;
        const int ContactInformationResponse = 34;
        const int UploadProfilePictureRequest = 35;
        const int UploadProfilePictureResponse = 36;
        const int UploadStatusRequest = 37;
        const int UploadStatusResponse = 38;
        const int ResetPasswordRequest = 30;
        const int ResetPasswordResponse = 31;
        const string loginResponse1 = "The login has been successfully completed";
        const string loginResponse2 = "The login has failed";
        const string loginResponse3 = "You need to change your password";
        const string loginResponse4 = "The login has been successfully completed but You haven't selected profile picture and status yet";
        const string loginResponse5 = "The login has been successfully completed but You haven't selected status yet";

        const string ResetPasswordResponse1 = "The username and email address were matching";
        const string ResetPasswordResponse2 = "The username and email address weren't matching";

        const string registerResponse1 = "Your registeration has completed successfully \nPlease press the back button to return to the home screen and login";
        const string registerResponse2 = "Your registeration has failed \nPlease try again ";
        const string colorResponse1 = "You have chosen the coolest color";
        const string colorResponse2 = "An error occurred to happen \nChoose a new Color";
        const string colorResponse3 = "Your oppoenent has already chosen this color \nPlease choose a diffrenet color";

        const string InitialProfileSettingsCheckResponse1 = "The login has been successfully completed but you need to change your password";
        const string InitialProfileSettingsCheckResponse2 = "The login has been successfully completed but You haven't selected profile picture and status yet";
        const string InitialProfileSettingsCheckResponse3 = "The login has been successfully completed but You haven't selected status yet";
        const string InitialProfileSettingsCheckResponse4 = "The login has been successfully completed";

        const string PasswordMessageResponse1 = "This password has already been chosen by you before";
        const string PasswordMessageResponse2 = "Your new password has been saved";
        const string PasswordMessageResponse3 = "An error occured";
        const string PasswordMessageResponse4 = "Your past details aren't matching";
        const string FriendRequestResponseSender1 = "Approval";
        const string FriendRequestResponseSender2 = "Rejection";
        public const string BanBeginning = "You are banned";
        public const string BanEnding = "Your ban is over";
        const string VideoCallResponse1 = "Your friend is offline. Please try to call again.";
        const string VideoCallResponse2 = "You have been asked to join a call";
        const string VideoCallResponseResult1 = "Joining the video call";
        const string VideoCallResponseResult2 = "Declining the video call";
        const string GroupCreatorResponse1 = "Group was successfully created";
        /// <summary>
        /// Represents rather the nickname has been sent
        /// </summary>
        private bool ReceiveNick = true;


        private RSAServiceProvider Rsa;
        private string ClientPublicKey;
        private string PrivateKey;

        private string SymmetricKey;

        /// <summary>
        /// The Client constructor initializes the client object, registers it with the server's client collection, and starts asynchronous data reading from the client
        /// </summary>
        /// <param name="client">Represents the client who connected to the server </param>
        public Client(TcpClient client)
        {
            Rsa = new RSAServiceProvider();
            PrivateKey = Rsa.GetPrivateKey();
            _client = client;
            Logger.LogUserLogIn("The user has established a connection to the server.");
            // get the ip address of the client to register him with our client list
            _clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            _clientIP = client.Client.RemoteEndPoint.ToString();
            // Add the new client to our clients collection
            AllClients.Add(_clientIP, this);
            clientsList.Add(this);

            // Read data from the client async
            data = new byte[_client.ReceiveBufferSize];

            // BeginRead will begin async read from the NetworkStream
            // This allows the server to remain responsive and continue accepting new connections from other clients
            // When reading complete control will be transfered to the ReviveMessage() function.
            _client.GetStream().BeginRead(data,
                                          0,
                                          System.Convert.ToInt32(_client.ReceiveBufferSize),
                                          ReceiveMessage,
                                          null);
        }

        public static string RandomKey(int Length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string (Enumerable.Repeat(chars, Length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }

    /// <summary>
    /// The ReceiveMessage method recieves and handles the incomming stream
    /// </summary>
    /// <param name="ar">IAsyncResult Interface</param>
    public void ReceiveMessage(IAsyncResult ar)
        {
            int bytesRead;
            try
            {
                lock (_client.GetStream())
                {
                    // call EndRead to handle the end of an async read.
                    bytesRead = _client.GetStream().EndRead(ar);
                }
                // if bytesread<1 -> the client disconnected
                if (bytesRead < 1)
                {
                    // remove the client from out list of clients
                    AllClients.Remove(_clientIP);
                    disconnectedClients.Enqueue(PlayerNum);
                    return;
                }
                else // client still connected
                {
                    string messageReceived = System.Text.Encoding.ASCII.GetString(data, 0, bytesRead);
                    string[] messageToArray = messageReceived.Split('$');
                    int requestNumber = Convert.ToInt32(messageToArray[0]);
                    string messageDetails = messageToArray[1];
                    string DecryptedMessageDetails;
                    // if the client is sending send me datatable
                    if (requestNumber == EncryptionClientPublicKeySender)
                    {
                        ClientPublicKey = messageDetails;
                        SendMessage(EncryptionServerPublicKeyReciever, Rsa.GetPublicKey());
                        //SendMessage(EncryptionServerPublicKeyReciever + "$" + Rsa.GetPublicKey());

                        SymmetricKey = RandomStringCreator.RandomString(32);
                        string EncryptedSymmerticKey = Rsa.Encrypt(SymmetricKey, ClientPublicKey);
                        SendMessage(EncryptionSymmetricKeyReciever, EncryptedSymmerticKey);
                        //SendMessage(EncryptionSymmetricKeyReciever + "$" + EncryptedSymmerticKey);

                    }
                    else
                    {
                        DecryptedMessageDetails = Encryption.Encryption.DecryptData(SymmetricKey, messageDetails);
                        if (requestNumber == registerRequest)
                        {
                            string[] data = DecryptedMessageDetails.Split('#');
                            if (!UserDetails.DataHandler.usernameIsExist(data[0]) /*&& !UserDetails.DataHandler.EmailAddressIsExist(data[4])*/)
                            {
                                if (UserDetails.DataHandler.InsertUser(DecryptedMessageDetails) > 0)
                                {
                                    _ClientNick = data[0];
                                    SendMessage(registerResponse, registerResponse1);
                                    //SendMessage(registerResponse + "$" + registerResponse1);

                                }
                                else//if regist not ok
                                {
                                    SendMessage(registerResponse, registerResponse2);
                                    //SendMessage(registerResponse + "$" + registerResponse2);

                                }
                            }
                            else//if regist not ok
                            {
                                SendMessage(registerResponse, registerResponse2);
                                //SendMessage(registerResponse + "$" + registerResponse2);

                            }
                        }
                        else if (requestNumber == loginRequest)
                        {
                            //string[] data = messageDetails.Split('#');
                            string[] data = DecryptedMessageDetails.Split('#');

                            //if ((UserDetails.DataHandler.isExist(messageDetails)) && (!UserIsConnected(data[0])))

                            if ((UserDetails.DataHandler.isExist(DecryptedMessageDetails)) && (!UserIsConnected(data[0])))
                            {
                                _ClientNick = data[0];
                                string emailAddress = UserDetails.DataHandler.GetEmailAddress(_ClientNick);
                                if (emailAddress != "")
                                {
                                    SendMessage(loginResponse, emailAddress);
                                    //SendMessage(loginResponse + "$" + emailAddress);

                                }
                                else
                                {
                                    //todo - send a message saying there was a problem
                                }
                            }
                            else
                            {
                                ClientAttemptsState clientAttemptsState;
                                if (!_clientFailedAttempts.ContainsKey(_clientAddress))
                                {
                                    clientAttemptsState = new ClientAttemptsState(this);
                                    _clientFailedAttempts[_clientAddress] = clientAttemptsState;
                                }
                                else
                                {
                                    clientAttemptsState = _clientFailedAttempts[_clientAddress];
                                }
                                clientAttemptsState.HandleFailedAttempt();
                                //if (_clientFailedAttempts.ContainsKey(_clientIP))
                                //{
                                //    _clientFailedAttempts[_clientIP]++;

                                //}
                                //else
                                //{
                                //    _clientFailedAttempts[_clientIP] = 1;

                                //}
                                //if (_clientFailedAttempts[_clientIP] > 5)
                                //{
                                //    //handle waiting..
                                //    // todo - handle block of spamming user and maybe do the following act:
                                //    //to send a message to the user saying his account got blocked for 10 minutes because someone tried to enter..
                                //}
                                SendMessage(loginResponse, loginResponse2);
                                //SendMessage(loginResponse + "$" + loginResponse2);

                            }
                        }
                        else if (requestNumber == sendMessageRequest)
                        {
                            string message = _ClientNick + "#" + DecryptedMessageDetails;
                            Multicast(sendMessageResponse, message);
                            //Broadcast(sendMessageResponse + "$" + message);

                        }
                        else if (requestNumber == ContactInformationRequest)
                        {
                            string ContactsInformation = "Dan" + "^" + "hi" + "^" + "07:50" + "^" + "Male3" + "#" + "Ben" + "^" + "how you doing" + "^" + "17:53" + "^" + "Female3 " + "#" + "Ron" + "^" + "YOO" + "^" + "03:43" + "^" + "Male4"; //בעתיד לקחת מידע מהdatabase
                            SendMessage(ContactInformationResponse, ContactsInformation);
                            //SendMessage(ContactInformationResponse + "$" + ContactsInformation);

                        }
                        else if ((requestNumber == UploadProfilePictureRequest))
                        {
                            if (UserDetails.DataHandler.InsertProfilePicture(_ClientNick, DecryptedMessageDetails) > 0)
                            {
                                SendMessage(UploadProfilePictureResponse, DecryptedMessageDetails);
                                //SendMessage(UploadProfilePictureResponse + "$" + registerResponse1);

                            }
                        }
                        else if ((requestNumber == UploadStatusRequest))
                        {
                            if (UserDetails.DataHandler.InsertStatus(_ClientNick, DecryptedMessageDetails) > 0)
                            {
                                SendMessage(UploadStatusResponse, DecryptedMessageDetails);
                                //SendMessage(UploadStatusResponse + "$" + registerResponse1);

                            }
                        }
                        else if (requestNumber == ResetPasswordRequest)
                        {
                            if (UserDetails.DataHandler.IsMatchingUsernameAndEmailAddressExist(DecryptedMessageDetails))
                            {
                                SendMessage(ResetPasswordResponse, ResetPasswordResponse1);
                                //SendMessage(ResetPasswordResponse + "$" + ResetPasswordResponse1);

                            }
                            else
                            {
                                SendMessage(ResetPasswordResponse, ResetPasswordResponse2);
                                //SendMessage(ResetPasswordResponse + "$" + ResetPasswordResponse2);

                            }
                        }
                        else if ((requestNumber == PasswordRenewalMessageRequest) || (requestNumber == PasswordUpdateRequest))
                        {
                            bool IsPasswordRenewalMessageRequest = (requestNumber == PasswordRenewalMessageRequest);
                            int identifierNumber; //maybe instead of handling both here i should write a method that get the idnumber and send the message accordinglly...
                            bool HasAccessToChange = true;
                            string[] data = DecryptedMessageDetails.Split('#');
                            string username = data[0];
                            string NewPassword = data[1];
                            if (IsPasswordRenewalMessageRequest)
                            {
                                identifierNumber = PasswordRenewalMessageResponse;
                            }
                            else
                            {
                                identifierNumber = PasswordUpdateResponse;

                            }
                            if (!IsPasswordRenewalMessageRequest)
                            {
                                string OldPassword = data[2];
                                if (!UserDetails.DataHandler.PasswordIsExist(username, OldPassword)) //means the password already been chosen once by the user...
                                {
                                    HasAccessToChange = false;
                                    SendMessage(identifierNumber, PasswordMessageResponse4); //past password not matching..

                                }
                            }
                            if (HasAccessToChange)
                            {
                                if (UserDetails.DataHandler.PasswordIsExist(username, NewPassword)) //means the password already been chosen once by the user...
                                {
                                    SendMessage(identifierNumber, PasswordMessageResponse1);

                                }
                                else
                                {
                                    if (UserDetails.DataHandler.CheckFullPasswordCapacity(username))
                                    {
                                        UserDetails.DataHandler.AddColumnToUserPastPasswords();
                                    }
                                    if (UserDetails.DataHandler.SetNewPassword(username, NewPassword) > 0)
                                    {
                                        SendMessage(identifierNumber, PasswordMessageResponse2);
                                        //SendMessage(PasswordRenewalMessageResponse + "$" + PasswordRenewalMessageResponse2);
                                    }
                                    else
                                    {
                                        SendMessage(identifierNumber, PasswordMessageResponse3);
                                        //SendMessage(PasswordRenewalMessageResponse + "$" + PasswordRenewalMessageResponse3);

                                    }
                                }
                            }

                        }
                        else if (requestNumber == InitialProfileSettingsCheckRequest)
                        {
                            if (IsNeededToUpdatePassword()) //opens the user the change password mode, he changes the password and if it's possible it automatticly let him enter or he needs to login once again...
                            {
                                SendMessage(InitialProfileSettingsCheckResponse, InitialProfileSettingsCheckResponse1);

                            }
                            else if (!UserDetails.DataHandler.ProfilePictureIsExist(_ClientNick)) //todo - change this - after doing the captcha i should ask the server for this information
                            {
                                SendMessage(InitialProfileSettingsCheckResponse, InitialProfileSettingsCheckResponse2);
                                //SendMessage(InitialProfileSettingsCheckResponse + "$" + loginResponse4);

                            }
                            else if (!UserDetails.DataHandler.StatusIsExist(_ClientNick))
                            {
                                SendMessage(InitialProfileSettingsCheckResponse, InitialProfileSettingsCheckResponse3);
                                //SendMessage(InitialProfileSettingsCheckResponse + "$" + loginResponse5);
                            }
                            else
                            {
                                SendMessage(InitialProfileSettingsCheckResponse, InitialProfileSettingsCheckResponse4);
                                //SendMessage(InitialProfileSettingsCheckResponse + "$" + loginResponse1);
                                if (UserDetails.DataHandler.SetUserOnline(_ClientNick) > 0)
                                {
                                    //was ok...
                                }

                            }
                        }
                        else if (requestNumber == UserDetailsRequest)
                        {
                            string UserInformation = UserDetails.DataHandler.GetUserProfileSettings(_ClientNick);
                            SendMessage(UserDetailsResponse, UserInformation);
                        }
                        else if (requestNumber == FriendRequestSender) //todo - needs to check if the user already sent a friend request before..
                        {
                            string[] data = DecryptedMessageDetails.Split('#');
                            string FriendRequestReceiverUsername = data[0];
                            string FriendRequestSenderUsername = _ClientNick;

                            if (FriendRequestSenderUsername != FriendRequestReceiverUsername)
                            {
                                if (UserDetails.DataHandler.IsMatchingUsernameAndTagLineIdExist(DecryptedMessageDetails))
                                {
                                    //ask friend request..
                                    if (UserDetails.DataHandler.AddFriendRequest(FriendRequestSenderUsername, FriendRequestReceiverUsername) > 0)
                                    {
                                        //was successful
                                        //to check if he is online...
                                        if (UserIsConnected(FriendRequestReceiverUsername))
                                        {
                                            string profilePicture = UserDetails.DataHandler.GetProfilePicture(FriendRequestSenderUsername);
                                            if (profilePicture != "")
                                            {
                                                string userDetails = FriendRequestSenderUsername + "^" + profilePicture;
                                                Unicast(FriendRequestReceiver, userDetails, FriendRequestReceiverUsername); //todo - need to handle in the client side how it will work
                                                                                                                            //need to handle when logging in if there were message request sent before...
                                            }

                                        }
                                    }
                                    else
                                    {
                                        //wasn't successful even though details were right - needs to inform the user and tell him to send once again...
                                    }

                                }
                            }

                        }
                        else if (requestNumber == FriendRequestResponseSender)
                        {
                            string[] data = DecryptedMessageDetails.Split('#'); //needs to string from the client the name of the user who sent and then the accept/deny...
                            string FriendRequestSenderUsername = data[0];
                            string FriendRequestReceiverUsername = _ClientNick;
                            string FriendRequestStatus = data[1];
                            if (UserDetails.DataHandler.HandleFriendRequestStatus(FriendRequestSenderUsername, FriendRequestReceiverUsername, FriendRequestStatus) > 0)
                            {
                                if (FriendRequestStatus == FriendRequestResponseSender1)
                                {
                                    if ((UserDetails.DataHandler.CheckFullFriendsCapacity(FriendRequestSenderUsername)) || (UserDetails.DataHandler.CheckFullFriendsCapacity(FriendRequestReceiverUsername)))
                                    {
                                        UserDetails.DataHandler.AddColumnToFriends();
                                    }
                                    if (UserDetails.DataHandler.AddFriend(FriendRequestSenderUsername, FriendRequestReceiverUsername) > 0) //one worked...
                                    {
                                        if (UserDetails.DataHandler.AddFriend(FriendRequestReceiverUsername, FriendRequestSenderUsername) > 0) //both worked...
                                        {
                                            Unicast(FriendRequestResponseReceiver, "the friend request has been accepted", FriendRequestSenderUsername);
                                        }
                                    }
                                    //if (UserDetails.DataHandler.setne(username, password) > 0)
                                    //{
                                    //    SendMessage(PasswordRenewalMessageResponse, PasswordRenewalMessageResponse2);
                                    //    //SendMessage(PasswordRenewalMessageResponse + "$" + PasswordRenewalMessageResponse2);
                                    //}
                                    //else
                                    //{
                                    //    SendMessage(PasswordRenewalMessageResponse, PasswordRenewalMessageResponse3);
                                    //    //SendMessage(PasswordRenewalMessageResponse + "$" + PasswordRenewalMessageResponse3);

                                    //}                                    //the user accepted the friend request and i should handle them being friends... both by entering to database and sending them message if they are connected so they will add one another in contacts..
                                }
                                else if (FriendRequestStatus == FriendRequestResponseSender2)
                                {
                                    // doesn't really need to do something... maybe in the future i will think abt something
                                }
                            }
                            else
                            {
                                //was an error...
                            }

                        }
                        else if (requestNumber == FriendsProfileDetailsRequest)
                        {
                            //string FriendsName = UserDetails.DataHandler.GetFriendList(_ClientNick);
                            //Dictionary<string, string> FriendsProfileDetailsDictionary = UserDetails.DataHandler.GetFriendsProfileInformation(FriendsName);
                            ////here i check the need for split messages...
                            //List<string> FriendsProfileDetails = new List<string>();
                            //int AllFriendProfileDetailsLength = 0;
                            //string FriendProfileDetails = "";
                            //int index = 0;
                            //foreach (KeyValuePair<string, string> kvp in FriendsProfileDetailsDictionary)
                            //{
                            //    FriendProfileDetails = "#" + kvp.Value;
                            //    byte[] currentUserDetailsBytes = Encoding.UTF8.GetBytes(FriendProfileDetails);

                            //    if (currentUserDetailsBytes.Length + AllFriendProfileDetailsLength + 4> 1500)      //needs to check if adding the content of the profiledetails will be two much length
                            //    {
                            //        index += 1;
                            //        AllFriendProfileDetailsLength = 0;
                            //    }
                            //    FriendsProfileDetails[index] += FriendProfileDetails;
                            //    AllFriendProfileDetailsLength += currentUserDetailsBytes.Length;
                            //}
                            //foreach (string FriendsProfileDetailsSet in FriendsProfileDetails)
                            //{
                            //    SendMessage(FriendsProfileDetailsResponse, FriendsProfileDetailsSet.Remove(0, 1)); //maybe i should split to couple of messages...
                            //}
                        }
                        else if (requestNumber == disconnectRequest)
                        {
                            if (_ClientNick != null)
                            {
                                if (UserDetails.DataHandler.SetUserOffline(_ClientNick) > 0)
                                {
                                    //was ok...
                                }
                            }
                        }
                        else if (requestNumber == PastFriendRequestsRequest)
                        {
                            string FriendRequestNamesOfSendersAndRequestDates = UserDetails.DataHandler.CheckFriendRequests(_ClientNick);
                            string[] FriendRequestDetails = FriendRequestNamesOfSendersAndRequestDates.Split('#');
                            string[] SplittedFriendRequestDetails;
                            string name;
                            string DetailsOfFriendRequestSenders = "";
                            //for (int index = FriendRequestDetails.Length - 1; index >= 0; index--)
                            //{
                            //    SplittedFriendRequestDetails = FriendRequestDetails[index].Split('^');
                            //    name = SplittedFriendRequestDetails[0];
                            //    string profilePicture = UserDetails.DataHandler.GetProfilePicture(name);
                            //    if (profilePicture != "")
                            //    {
                            //        DetailsOfFriendRequestSenders = FriendRequestDetails[index] + "^" + profilePicture + "#";
                            //    }
                            //}
                            foreach (string friendRequestDetails in FriendRequestDetails)
                            {
                                SplittedFriendRequestDetails = friendRequestDetails.Split('^');
                                name = SplittedFriendRequestDetails[0];
                                string profilePicture = UserDetails.DataHandler.GetProfilePicture(name);
                                if (profilePicture != "")
                                {
                                    DetailsOfFriendRequestSenders += friendRequestDetails + "^" + profilePicture + "#";
                                }
                            }
                            if (DetailsOfFriendRequestSenders.EndsWith("#"))
                            {
                                DetailsOfFriendRequestSenders = DetailsOfFriendRequestSenders.Substring(0, DetailsOfFriendRequestSenders.Length - 1);
                            }
                            SendMessage(PastFriendRequestsResponse, DetailsOfFriendRequestSenders);

                        }
                        else if (requestNumber == VideoCallRequest)
                        {
                            string friendName = DecryptedMessageDetails;
                            if ((UserIsConnected(friendName)) && (DataHandler.StatusIsExist(friendName)))
                            {
                                //establish a udp connection between them two and the server...
                                string messageContent = VideoCallResponse2 + "#" + _ClientNick;
                                Unicast(VideoCallResponse, messageContent, friendName); //what if he is currently in a call? will it work //todo - needs to check that in the future
                            }
                            else
                            {
                                SendMessage(VideoCallResponse, VideoCallResponse1);

                            }
                        }
                        else if (requestNumber == VideoCallResponseSender)
                        {
                            string[] messageContent = DecryptedMessageDetails.Split('#');
                            string messageInformation = messageContent[0];
                            string friendName = messageContent[1];
                            if (messageInformation == VideoCallResponseResult1)
                            {
                                Unicast(VideoCallResponseReciever, VideoCallResponseResult1, friendName);
                                //needs to create the udp connection...

                            }
                            else //the call wont happen...
                            {
                                Unicast(VideoCallResponseReciever, VideoCallResponseResult2, friendName);

                            }
                        }
                        else if (requestNumber == GroupCreatorRequest)
                        {
                            ChatCreator newChat = JsonConvert.DeserializeObject<ChatCreator>(DecryptedMessageDetails);
                            if (DataHandler.CreateGroupChat(newChat) > 0)
                            {
                                SendMessage(GroupCreatorResponse, "Group was successfully created");
                                List<string> chatMembers = newChat._chatParticipants;
                                chatMembers.RemoveAt(0);
                                ChatMembersCast(GroupCreatorResponse, DecryptedMessageDetails, chatMembers);
                                //needs to send this group creation to every logged user...
                            }

                        }
                    }

                    

                }
                lock (_client.GetStream())
                {
                    // continue reading from the client
                    _client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(_client.ReceiveBufferSize), ReceiveMessage, null);
                }
            }
            catch (Exception ex)
            {
                AllClients.Remove(_clientIP);
                //Broadcast(_ClientNick + " has left the chat.");

            }
        }//end ReceiveMessage

        /// <summary>
        /// The SendMessage method sends a message to the connected client
        /// </summary>
        /// <param name="message">Represents the message the server sends to the connected client</param>
        public void SendMessage(int messageId, string messageContent)
        {
            try
            {
                System.Net.Sockets.NetworkStream ns;
                // we use lock to present multiple threads from using the networkstream object
                // this is likely to occur when the server is connected to multiple clients all of 
                // them trying to access to the networkstram at the same time.
                lock (_client.GetStream())
                {
                    ns = _client.GetStream();
                }
                // Send data to the client
                string message;
                if ((messageId == EncryptionClientPublicKeySender) || (messageId == EncryptionSymmetricKeyReciever))
                {
                    message = messageId + "$" + messageContent;
                }
                else
                {
                    string EncryptedMessageContent = Encryption.Encryption.EncryptData(SymmetricKey, messageContent);
                    message = messageId + "$" + EncryptedMessageContent;
                }
                byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);
                ns.Write(bytesToSend, 0, bytesToSend.Length);
                ns.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }//end SendMessage
        //public void SendMessage(string message)
        //{
        //    try
        //    {
        //        System.Net.Sockets.NetworkStream ns;
        //        // we use lock to present multiple threads from using the networkstream object
        //        // this is likely to occur when the server is connected to multiple clients all of 
        //        // them trying to access to the networkstram at the same time.
        //        lock (_client.GetStream())
        //        {
        //            ns = _client.GetStream();
        //        }
        //        // Send data to the client
        //        byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(message);
        //        ns.Write(bytesToSend, 0, bytesToSend.Length);
        //        ns.Flush();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}//end SendMessage


        public bool IsNeededToUpdatePassword()
        {
            bool NeededToUpdatePassword = false;
            DateTime LastPasswordUpdateDate = UserDetails.DataHandler.GetLastPasswordUpdateDate(_ClientNick);
            DateTime CurrentDate = DateTime.Now;  // Replace with your actual end date
            int MonthsDifference = CalculateMonthsDifference(LastPasswordUpdateDate, CurrentDate);
            if (MonthsDifference >= 1)
            {
                NeededToUpdatePassword = true;
            }
            return NeededToUpdatePassword;
        }

        public static int CalculateMonthsDifference(DateTime StartDate, DateTime EndDate)
        {
            if (StartDate >= EndDate)
            {
                return 0;
            }
            int YearDifference = (EndDate.Year - StartDate.Year);
            int MonthDifference = (EndDate.Month - StartDate.Month);

            int monthsApart = 12 * YearDifference + MonthDifference;

            // Adjust for cases where the endDate's day is earlier than the startDate's day
            if (EndDate.Day < StartDate.Day)
            {
                monthsApart--;
            }

            return monthsApart;
        }
    


        /// <summary>
        /// The UserIsConnected method checks if a user with the given username is connected by searching for a matching username in the AllClients dictionary
        /// </summary>
        /// <param name="username">Represents the name of the player who tries to login</param>
        /// <returns>It returns true if the username is already connected. OtherWise, it returns false </returns>
        public Boolean UserIsConnected(string username)
        {
            foreach (DictionaryEntry c in AllClients)
            {
                if (((Client)(c.Value))._ClientNick == username)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// The Broadcast method sends the message to every client in the AllClients dictionary
        /// </summary>
        /// <param name="message">Represents the message the server is sending to every client</param>
        public void Broadcast(int messageId, string messageContent)
        {
            foreach (DictionaryEntry c in AllClients)
            {
                ((Client)(c.Value)).SendMessage(messageId, messageContent);
            }
        }

        public void Multicast(int messageId, string messageContent)
        {
            foreach (DictionaryEntry c in AllClients)
            {
                if (!((Client)(c.Value))._ClientNick.Equals(this._ClientNick))
                {
                    ((Client)(c.Value)).SendMessage(messageId, messageContent);
                }
            }
        }
        public void Unicast(int messageId, string messageContent, string UserID)
        {
            foreach (DictionaryEntry c in AllClients)
            {
                if (((Client)(c.Value))._ClientNick == UserID) //בעתיד להחליף clientnick במשתנה של userid
                {
                    ((Client)(c.Value)).SendMessage(messageId, messageContent);
                }
            }
        }
        public void ChatMembersCast(int messageId, string messageContent, List<string> chatMembers)
        {
            foreach (DictionaryEntry c in AllClients)
            {
                foreach (string ChatMamber in chatMembers)
                {
                    if (((Client)(c.Value))._ClientNick == ChatMamber)
                    {
                        ((Client)(c.Value)).SendMessage(messageId, messageContent);
                    }
                }
            }
        }
        ///// <summary>
        ///// The Broadcast method sends the message to every client in the AllClients dictionary
        ///// </summary>
        ///// <param name="message">Represents the message the server is sending to every client</param>
        //public void Broadcast(string message)
        //{
        //    foreach (DictionaryEntry c in AllClients)
        //    {
        //        ((Client)(c.Value)).SendMessage(message);
        //    }
        //}

        //public void Multicast(string message)
        //{
        //    foreach (DictionaryEntry c in AllClients)
        //    {
        //        if (!((Client)(c.Value))._ClientNick.Equals(this._ClientNick))
        //        {
        //            ((Client)(c.Value)).SendMessage(message);
        //        }
        //    }
        //}
        //public void Unicast(string message, string UserID)
        //{
        //    foreach (DictionaryEntry c in AllClients)
        //    {
        //        if (((Client)(c.Value))._ClientNick == UserID) //בעתיד להחליף clientnick במשתנה של userid
        //        {
        //            ((Client)(c.Value)).SendMessage(message);
        //        }
        //    }
        //}

        public string getCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 5).Select(s =>
            s[random.Next(s.Length)]).ToArray());
        }

        private void ReceiveImageUDP()
        {
            UdpClient udpListener = new UdpClient(12345); // Use the same port as the client

            while (true)
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] imageData = udpListener.Receive(ref clientEndPoint);

                // Convert bytes to image
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    Image receivedImage = Image.FromStream(ms);
                }
            }
        }

    }
}
