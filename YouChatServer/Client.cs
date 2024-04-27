using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using YouChatServer.Encryption;
using Image = System.Drawing.Image;
using YouChatServer.UserDetails;
using Newtonsoft.Json;
using YouChatServer.ChatHandler;
using YouChatServer.JsonClasses;
using YouChatServer.CaptchaHandler;
using YouChatServer.ClientAttemptsStateHandler;
using YouChatServer.ContactHandler;
using YouChatServer.UdpHandler;
using YouChatServer.JsonClasses.MessageClasses;
using Newtonsoft.Json.Bson;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Windows.Forms;

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

        private ClientAttemptsState _LoginFailedAttempts;
        private ClientAttemptsState _RegistrationFailedAttempts;
        private ClientAttemptsState _RegistrationSmtpFailedAttempts;

        private ClientAttemptsState _PasswordUpdateFailedAttempts;
        private ClientAttemptsState _PasswordResetFailedAttempts;

        private ClientCaptchaRotationImagesAttemptsState _captchaRotationImagesAttemptsState;
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
        private IPEndPoint _clientIPEndPoint;

        private byte[] dataHistory;

        /// <summary>
        /// Represents the client username
        /// </summary>
        private string _ClientNick;

        private bool _isOnline;
        private bool _inCall;

        /// <summary>
        /// Byte array which represents the data received from the client
        /// It it used for both sending and reciving data
        /// </summary>
        private byte[] data;

        static Random Random = new Random();

        private bool isClientConnected;
      
        const string ApprovalFriendRequestResponse = "Approval";
        const string RejectionFriendRequestResponse = "Rejection";


        private RSAServiceProvider Rsa;
        private string ClientPublicKey;
        private string PrivateKey;

        private string SymmetricKey;
        private SmtpHandler smtpHandler;
        private EncryptionExpirationDate encryptionExpirationDate;
        private CaptchaCodeHandler captchaCodeHandler;
        private CaptchaRotatingImageHandler captchaRotatingImageHandler;
        /// <summary>
        /// The Client constructor initializes the client object, registers it with the server's client collection, and starts asynchronous data reading from the client
        /// </summary>
        /// <param name="client">Represents the client who connected to the server </param>
        public Client(TcpClient client)
        {
            _client = client;
            Logger.LogUserLogIn("A user has established a connection to the server.");
            // get the ip address of the client to register him with our client list
            _clientIPEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            _clientAddress = _clientIPEndPoint.Address;
            _clientIP = client.Client.RemoteEndPoint.ToString();
            // Add the new client to our clients collection
            AllClients.Add(_clientIP, this);
            clientsList.Add(this);

            // Read data from the client async
            data = new byte[_client.ReceiveBufferSize];
            dataHistory = new byte[0];

            // BeginRead will begin async read from the NetworkStream
            // This allows the server to remain responsive and continue accepting new connections from other clients
            // When reading complete control will be transfered to the ReviveMessage() function.
            //_client.GetStream().BeginRead(data,
            //                              0,
            //                              System.Convert.ToInt32(_client.ReceiveBufferSize),
            //                              ReceiveMessage,
            //                              null);
            _client.GetStream().BeginRead(data,
                              0,
                              4,
                              ReceiveMessageLength,
                              null);

            isClientConnected = true;
            smtpHandler = new SmtpHandler();
            encryptionExpirationDate = new EncryptionExpirationDate(this);
            captchaCodeHandler = new CaptchaCodeHandler();
            captchaRotatingImageHandler = new CaptchaRotatingImageHandler();
            _LoginFailedAttempts = new ClientAttemptsState(this,EnumHandler.UserAuthentication_Enum.Login);
            _isOnline = false;
            _inCall = false;
            //ClientAttemptsState clientAttemptsState = null;
            //InitializeClientAttemptsStateObject(ref clientAttemptsState);

        }
        private void HandleEncryptionClientPublicKeySenderEnum(JsonObject jsonObject)
        {
            ClientPublicKey = jsonObject.MessageBody as string;


            //string serverPublicKey = Rsa.GetPublicKey();
            //SymmetricKey = RandomStringCreator.RandomString(32);
            //string EncryptedSymmerticKey = Rsa.Encrypt(SymmetricKey, ClientPublicKey);
            //EncryptionKeys encryptionKeys = new EncryptionKeys(EncryptedSymmerticKey, serverPublicKey);
            //JsonObject serverPublicKeyJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.EncryptionServerPublicKeyAndSymmetricKeyReciever, encryptionKeys);
            //string serverPublicKeyJson = JsonConvert.SerializeObject(serverPublicKeyJsonObject, new JsonSerializerSettings
            //{
            //    TypeNameHandling = TypeNameHandling.Auto
            //});
            //SendMessage(serverPublicKeyJson, false);
            Rsa = new RSAServiceProvider();
            PrivateKey = Rsa.GetPrivateKey();
            string serverPublicKey = Rsa.GetPublicKey();
            JsonObject serverPublicKeyJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.EncryptionServerPublicKeyReciever, serverPublicKey);
            string serverPublicKeyJson = JsonConvert.SerializeObject(serverPublicKeyJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            Console.WriteLine("print");
            Console.WriteLine(serverPublicKeyJson);
            SendMessage(serverPublicKeyJson, false);
            SymmetricKey = RandomStringCreator.RandomString(32);
            string EncryptedSymmerticKey = Rsa.Encrypt(SymmetricKey, ClientPublicKey);
            JsonObject encryptedSymmerticKeyJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.EncryptionSymmetricKeyReciever, EncryptedSymmerticKey);
            string EncryptedSymmerticKeyJson = JsonConvert.SerializeObject(encryptedSymmerticKeyJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            Console.WriteLine("print");
            Console.WriteLine(EncryptedSymmerticKeyJson);
            //Thread.Sleep(1000);
            SendMessage(EncryptedSymmerticKeyJson, false);
            encryptionExpirationDate.Start();
        }
        private void HandleLoginRequestEnum(JsonObject jsonObject)
        {
            LoginDetails loginDetails = jsonObject.MessageBody as LoginDetails;
            string username = loginDetails.Username;
            string password = loginDetails.Password;
            if ((DataHandler.isMatchingUsernameAndPasswordExist(username, password)) && (!UserIsConnected(username)))
            {
                _ClientNick = username;


                if (DataHandler.SetUserOnline(_ClientNick) > 0)
                {
                    _isOnline = true;
                    JsonObject personalVerificationAnswersResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_OpenChat, null);
                    string personalVerificationAnswersResponseJson = JsonConvert.SerializeObject(personalVerificationAnswersResponseJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    SendMessage(personalVerificationAnswersResponseJson);
                }


                //string emailAddress = DataHandler.GetEmailAddress(_ClientNick);
                //if (emailAddress != "")
                //{
                //    smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.LoginMessage);
                //    //ClientAttemptsState clientAttemptsState = null;
                //    //InitializeClientAttemptsStateObject(ref clientAttemptsState);
                //    _LoginFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Login);
                //    JsonObject smtpLoginMessageJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.loginResponse_SmtpLoginMessage, null);
                //    string smtpLoginMessageJson = JsonConvert.SerializeObject(smtpLoginMessageJsonObject, new JsonSerializerSettings
                //    {
                //        TypeNameHandling = TypeNameHandling.Auto
                //    });
                //    SendMessage(smtpLoginMessageJson);
                //}
                //else //shouldn't get here - emailaddress won't be empty...
                //{
                //    SendFailedLoginMessage();
                //}
            }
            else
            {
                //ClientAttemptsState clientAttemptsState = GetClientAttemptsStateObject();
                //HandleFailedAttempt(clientAttemptsState, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SendFailedLoginMessage);
                HandleFailedAttempt(_LoginFailedAttempts, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SendFailedLoginMessage);
            }
        }
        private void HandleFailedAttempt(ClientAttemptsState clientAttemptsState, EnumHandler.CommunicationMessageID_Enum BanStart, Action FailedAttempt)
        {
            clientAttemptsState.HandleFailedAttempt();
            if (clientAttemptsState.IsUserBanned())
            {
                clientAttemptsState.HandleBan();
                Logger.LogUserLogOut("A user has been blocked from the server.");
                double banDuration = clientAttemptsState.CurrentBanDuration;
                JsonObject banMessageJsonObject = new JsonObject(BanStart, banDuration);
                string banMessageJson = JsonConvert.SerializeObject(banMessageJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(banMessageJson);
            }
            else
            {
                FailedAttempt();
            }
        }
        private void HandleFailedAttempt(ClientAttemptsState clientAttemptsState, EnumHandler.CommunicationMessageID_Enum BanStart, string username, Action<string> FailedAttempt)
        {
            clientAttemptsState.HandleFailedAttempt();
            if (clientAttemptsState.IsUserBanned())
            {
                clientAttemptsState.HandleBan();
                Logger.LogUserLogOut("A user has been blocked from the server.");
                double banDuration = clientAttemptsState.CurrentBanDuration;
                JsonObject banMessageJsonObject = new JsonObject(BanStart, banDuration);
                string banMessageJson = JsonConvert.SerializeObject(banMessageJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(banMessageJson);
            }
            else
            {
                FailedAttempt(username);
            }
        }
        private void HandleFailedAttempt(ClientAttemptsState clientAttemptsState, EnumHandler.CommunicationMessageID_Enum BanStart, string username, string emailAddress, Action<string,string> FailedAttempt)
        {
            clientAttemptsState.HandleFailedAttempt();
            if (clientAttemptsState.IsUserBanned())
            {
                clientAttemptsState.HandleBan();
                Logger.LogUserLogOut("A user has been blocked from the server.");
                double banDuration = clientAttemptsState.CurrentBanDuration;
                JsonObject banMessageJsonObject = new JsonObject(BanStart, banDuration);
                string banMessageJson = JsonConvert.SerializeObject(banMessageJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(banMessageJson);
            }
            else
            {
                FailedAttempt(username, emailAddress);
            }
        }
        private void HandleFailedAttempt(ClientAttemptsState clientAttemptsState, EnumHandler.CommunicationMessageID_Enum BanStartEnum, EnumHandler.CommunicationMessageID_Enum FailedAttemptEnum, Action<EnumHandler.CommunicationMessageID_Enum> FailedAttempt)
        {
            clientAttemptsState.HandleFailedAttempt();
            if (clientAttemptsState.IsUserBanned())
            {
                clientAttemptsState.HandleBan();
                Logger.LogUserLogOut("A user has been blocked from the server.");
                double banDuration = clientAttemptsState.CurrentBanDuration;
                JsonObject banMessageJsonObject = new JsonObject(BanStartEnum, banDuration);
                string banMessageJson = JsonConvert.SerializeObject(banMessageJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(banMessageJson);
            }
            else
            {
                FailedAttempt(FailedAttemptEnum);
            }
        }
        //private ClientAttemptsState GetClientAttemptsStateObject()
        //{
        //    ClientAttemptsState clientAttemptsState = null;
        //    if (!_clientLoginFailedAttempts.ContainsKey(_clientIPEndPoint)) //shouldnt enter but for saftey...
        //    {
        //        InitializeClientAttemptsStateObject(ref clientAttemptsState);
        //    }
        //    else
        //    {
        //        clientAttemptsState = _clientLoginFailedAttempts[_clientIPEndPoint];
        //    }
        //    return clientAttemptsState;
        //}
        //private void InitializeClientAttemptsStateObject(ref ClientAttemptsState clientAttemptsState)
        //{
        //    clientAttemptsState = new ClientAttemptsState(this);
        //    _clientLoginFailedAttempts[_clientIPEndPoint] = clientAttemptsState;
        //}
        private void HandleloginRequest_SmtpLoginMessage(JsonObject jsonObject)
        {
            SmtpVerification smtpVerification = jsonObject.MessageBody as SmtpVerification;
            string username = smtpVerification.Username;
            bool afterFail = smtpVerification.AfterFail;
            if (afterFail)
            {
                HandleSmtpLoginMessage(username);
            }
            else
            {
                HandleFailedAttempt(_LoginFailedAttempts, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, username, HandleSmtpLoginMessage);
            }
        }
        private void HandleSmtpLoginMessage(string username)
        {
            string emailAddress = DataHandler.GetEmailAddress(_ClientNick);
            smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.LoginMessage);
            JsonObject smtpLoginMessageJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.loginResponse_SmtpLoginMessage, null);
            string smtpLoginMessageJson = JsonConvert.SerializeObject(smtpLoginMessageJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(smtpLoginMessageJson);
        }

        private void HandleRegistrationRequest_SmtpRegistrationCodeEnum(JsonObject jsonObject)
        {
            string code = jsonObject.MessageBody as string;
            if (smtpHandler.GetSmtpCode() == code)
            {
                _RegistrationSmtpFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Registration);
                JsonObject smtpRegistrationCodeResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_SuccessfulSmtpRegistrationCode, null);
                string smtpRegistrationCodeResponseJson = JsonConvert.SerializeObject(smtpRegistrationCodeResponseJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(smtpRegistrationCodeResponseJson);
            }
            else
            {
                if (_RegistrationSmtpFailedAttempts == null)
                {
                    _RegistrationSmtpFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Registration);
                }
                HandleFailedAttempt(_RegistrationSmtpFailedAttempts, EnumHandler.CommunicationMessageID_Enum.RegistrationBanStart, SendFailedSmtpRegistrationCode);
            }

        }
        private void SendFailedSmtpRegistrationCode()
        {
            JsonObject smtpRegistrationCodeResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_FailedSmtpRegistrationCode, null);
            string smtpRegistrationCodeResponseJson = JsonConvert.SerializeObject(smtpRegistrationCodeResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(smtpRegistrationCodeResponseJson);
        }
        private void SendFailedSmtpPasswordRestartCode()
        {
            JsonObject smtpResetPasswordCodeResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.FailedResetPasswordResponse_SmtpCode, null);
            string smtpResetPasswordCodeResponseJson = JsonConvert.SerializeObject(smtpResetPasswordCodeResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(smtpResetPasswordCodeResponseJson);
        }
        private void HandleResetPasswordRequest_SmtpCodeEnum(JsonObject jsonObject)
        {
            string code = jsonObject.MessageBody as string;
            if (smtpHandler.GetSmtpCode() == code)
            {
                _PasswordResetFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.PasswordRestart);
                JsonObject smtpResetPasswordCodeResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.SuccessfulResetPasswordResponse_SmtpCode, null);
                string smtpResetPasswordCodeResponseJson = JsonConvert.SerializeObject(smtpResetPasswordCodeResponseJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(smtpResetPasswordCodeResponseJson);
            }
            else
            {
                if (_PasswordResetFailedAttempts == null)
                {
                    _PasswordResetFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.PasswordRestart);
                }
                HandleFailedAttempt(_PasswordResetFailedAttempts, EnumHandler.CommunicationMessageID_Enum.ResetPasswordBanStart, SendFailedSmtpPasswordRestartCode);
            }
        }
        private void HandleResetPasswordRequestEnum(JsonObject jsonObject)
        {
            SmtpDetails smtpDetails = jsonObject.MessageBody as SmtpDetails;
            SmtpVerification smtpVerification = smtpDetails.SmtpVerification;
            string username = smtpVerification.Username;
            string emailAddress = smtpDetails.EmailAddress;
            if (DataHandler.IsMatchingUsernameAndEmailAddressExist(username, emailAddress))
            {
                _PasswordResetFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.PasswordRestart);
                smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.PasswordRenewalMessage);
                JsonObject resetPasswordResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.SuccessfulResetPasswordResponse, null);
                string resetPasswordResponseJson = JsonConvert.SerializeObject(resetPasswordResponseJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(resetPasswordResponseJson);
            }
            else
            {

                if (_PasswordResetFailedAttempts == null)
                {
                    _PasswordResetFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.PasswordRestart);
                }
                HandleFailedAttempt(_PasswordResetFailedAttempts, EnumHandler.CommunicationMessageID_Enum.ResetPasswordBanStart, SendFailedResetPasswordResponse);
            }
        }
        private void SendFailedResetPasswordResponse()
        {
            JsonObject resetPasswordResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.FailedResetPasswordResponse, null);
            string resetPasswordResponseJson = JsonConvert.SerializeObject(resetPasswordResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(resetPasswordResponseJson);
        }

        private void HandleFriendRequestSenderEnum(JsonObject jsonObject) //todo - not finished...
        {
            FriendRequestDetails friendRequestDetails = jsonObject.MessageBody as FriendRequestDetails;
            string FriendRequestReceiverUsername = friendRequestDetails.Username;
            string FriendRequestReceiverTagLine = friendRequestDetails.TagLine;
            string FriendRequestSenderUsername = _ClientNick;

            if (FriendRequestSenderUsername != FriendRequestReceiverUsername)
            {
                if (UserDetails.DataHandler.IsMatchingUsernameAndTagLineIdExist(FriendRequestReceiverUsername, FriendRequestReceiverTagLine)) //if it's wrong i chose not to inform because he shouldnt know if there wasn't a client like that (this way he could find other clients...)
                {
                    if (!DataHandler.AreFriends(FriendRequestSenderUsername, FriendRequestReceiverUsername))
                    {
                        if (DataHandler.IsFriendRequestPending(FriendRequestSenderUsername, FriendRequestReceiverUsername)) //user already send one
                        {

                        }
                        else if (DataHandler.IsFriendRequestPending(FriendRequestReceiverUsername, FriendRequestSenderUsername)) //the other user already send to you
                        {
                            //handle accept request...
                            HandleFriendRequestResponse(FriendRequestReceiverUsername, FriendRequestSenderUsername, ApprovalFriendRequestResponse);
                        }
                        else
                        {
                            if (UserDetails.DataHandler.AddFriendRequest(FriendRequestSenderUsername, FriendRequestReceiverUsername) > 0)
                            {
                                //was successful
                                string emailAddress = DataHandler.GetEmailAddress(FriendRequestReceiverUsername);
                                //todo - check if it works...
                                smtpHandler.SendFriendRequestAlertToUserEmail(FriendRequestReceiverUsername, FriendRequestSenderUsername, emailAddress); //sends if he is offline so he know and if he is online so he will know to look there...

                                //to check if he is online...
                                string profilePicture = DataHandler.GetProfilePicture(FriendRequestSenderUsername);
                                DateTime currentTime = DateTime.Now;
                                DateTime requestDate = DataHandler.GetFriendRequestDate(FriendRequestSenderUsername, FriendRequestReceiverUsername, currentTime);

                                if (profilePicture != "" && requestDate != currentTime)
                                {
                                    PastFriendRequest friendRequest = new PastFriendRequest(FriendRequestSenderUsername, profilePicture, requestDate);
                                    JsonObject friendRequestJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.FriendRequestReciever, friendRequest);
                                    string friendRequestJson = JsonConvert.SerializeObject(friendRequestJsonObject, new JsonSerializerSettings
                                    {
                                        TypeNameHandling = TypeNameHandling.Auto
                                    });
                                    Unicast(friendRequestJson, FriendRequestReceiverUsername);
                                    //todo - need to handle in the client side how it will work
                                    //need to handle when logging in if there were message request sent before...
                                }
                            }
                            else
                            {
                                //wasn't successful even though details were right - needs to inform the user and tell him to send once again...
                            }
                        }
                    }
                    else
                    {
                        //currently friends...
                    }
                }
            }
        }
        private void HandleFriendRequestResponseSenderEnum(JsonObject jsonObject) //todo - not finished...
        {
            FriendRequestResponseDetails friendRequestResponseDetails = jsonObject.MessageBody as FriendRequestResponseDetails;
            string FriendRequestSenderUsername = friendRequestResponseDetails.Username;
            string FriendRequestReceiverUsername = _ClientNick;
            string FriendRequestStatus = friendRequestResponseDetails.Status;
            HandleFriendRequestResponse(FriendRequestSenderUsername, FriendRequestReceiverUsername, FriendRequestStatus);
        }
        private void HandleFriendRequestResponse(string FriendRequestSenderUsername, string FriendRequestReceiverUsername, string FriendRequestStatus)
        {
            if (DataHandler.HandleFriendRequestStatus(FriendRequestSenderUsername, FriendRequestReceiverUsername, FriendRequestStatus) > 0)
            {
                if (FriendRequestStatus == ApprovalFriendRequestResponse)
                {
                    if ((DataHandler.CheckFullFriendsCapacity(FriendRequestSenderUsername)) || (DataHandler.CheckFullFriendsCapacity(FriendRequestReceiverUsername)))
                    {
                        DataHandler.AddColumnToFriends();
                    }

                    List<string> chatParticipantNames = new List<string>
                    {
                        FriendRequestSenderUsername,
                        FriendRequestReceiverUsername
                    };
                    string ChatTagLine = DataHandler.SetTagLine("GroupChats", "DirectChats");
                    string xmlFileName = $"DirectChat - {ChatTagLine} - {FriendRequestSenderUsername} AND {FriendRequestReceiverUsername}";
                    XmlFileManager xmlFileManager = new XmlFileManager(xmlFileName, chatParticipantNames, ChatTagLine); //maybe i should create it before i do handledirectchatcreation and if it dont work to delete it...
                    string filePath = xmlFileManager.GetFilePath();
                    if (DataHandler.HandleDirectChatCreation(ChatTagLine, FriendRequestSenderUsername, FriendRequestReceiverUsername, filePath))
                    {
                        List<ChatParticipant> chatParticipants = DataHandler.GetChatParticipants(chatParticipantNames);
                        ChatHandler.ChatDetails chat = new DirectChatDetails(ChatTagLine, null, "", "", chatParticipants);
                        ChatHandler.ChatHandler.AllChats.Add(ChatTagLine, chat);
                        ChatHandler.ChatHandler.ChatFileManagers.Add(ChatTagLine, xmlFileManager);

                        DirectChatDetails directChat = (DirectChatDetails)chat;

                        ContactDetails friendRequestSenderUsernameContact = DataHandler.GetFriendProfileInformation(FriendRequestSenderUsername);
                        ContactAndChat friendRequestSenderUsernameContactAndChat = new ContactAndChat(directChat, friendRequestSenderUsernameContact);
                        JsonObject friendRequestSenderUsernameContactAndChatJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.FriendRequestResponseReciever, friendRequestSenderUsernameContactAndChat);
                        string friendRequestSenderUsernameContactAndChatJson = JsonConvert.SerializeObject(friendRequestSenderUsernameContactAndChatJsonObject, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });
                        SendMessage(friendRequestSenderUsernameContactAndChatJson);


                        ContactDetails friendRequestReceiverUsernameContact = DataHandler.GetFriendProfileInformation(FriendRequestReceiverUsername);
                        ContactAndChat friendRequestReceiverUsernameContactAndChat = new ContactAndChat(directChat, friendRequestReceiverUsernameContact);

                        JsonObject friendRequestReceiverUsernameContactAndChatJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.FriendRequestResponseReciever, friendRequestReceiverUsernameContactAndChat);
                        string friendRequestReceiverUsernameContactAndChatJson = JsonConvert.SerializeObject(friendRequestReceiverUsernameContactAndChatJsonObject, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });
                        Unicast(friendRequestReceiverUsernameContactAndChatJson, FriendRequestSenderUsername);
                    }
                    else
                    {
                        xmlFileManager.DeleteFile();
                    }
                    //the user accepted the friend request and i should handle them being friends... both by entering to database and sending them message if they are connected so they will add one another in contacts..
                }
                else if (FriendRequestStatus == RejectionFriendRequestResponse)
                {
                    // doesn't really need to do something... maybe in the future i will think abt something
                }
            }
            else
            {
                //was an error...
            }
        }
        private void HandleRegistrationRequest_SmtpRegistrationMessageEnum(JsonObject jsonObject) 
        {
            SmtpDetails userUsernameAndEmailAddress = jsonObject.MessageBody as SmtpDetails;
            SmtpVerification smtpVerification = userUsernameAndEmailAddress.SmtpVerification;
            string username = smtpVerification.Username;
            bool afterFail = smtpVerification.AfterFail;
            string emailAddress = userUsernameAndEmailAddress.EmailAddress;
            if (afterFail)
            {
                SmtpRegistrationMessage(username,emailAddress);
            }
            else
            {
                if (_RegistrationSmtpFailedAttempts == null)
                {
                    _RegistrationSmtpFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Registration);
                }
                HandleFailedAttempt(_RegistrationSmtpFailedAttempts, EnumHandler.CommunicationMessageID_Enum.RegistrationBanStart, username, emailAddress, SmtpRegistrationMessage);

            }
        }

        private void SmtpRegistrationMessage(string username, string emailAddress)
        {
            smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.RegistrationMessage);
            JsonObject SentEmailNotificationJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_SmtpRegistrationMessage, null);
            string SentEmailNotificationJson = JsonConvert.SerializeObject(SentEmailNotificationJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(SentEmailNotificationJson);
        }
        private void HandleSmtpResetPasswordMessage(string username, string emailAddress)
        {
            smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.PasswordRenewalMessage);
            JsonObject SentEmailNotificationJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.ResetPasswordResponse_SmtpMessage, null);
            string SentEmailNotificationJson = JsonConvert.SerializeObject(SentEmailNotificationJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(SentEmailNotificationJson);
        }
        private void HandleResetPasswordRequest_SmtpMessageEnum(JsonObject jsonObject)
        {
            SmtpDetails smtpDetails = jsonObject.MessageBody as SmtpDetails;
            SmtpVerification smtpVerification = smtpDetails.SmtpVerification;
            string username = smtpVerification.Username;
            bool afterFail = smtpVerification.AfterFail;
            string emailAddress = smtpDetails.EmailAddress;

            if (afterFail)
            {
                HandleSmtpResetPasswordMessage(username,emailAddress);
            }
            else
            {
                HandleFailedAttempt(_PasswordResetFailedAttempts, EnumHandler.CommunicationMessageID_Enum.ResetPasswordBanStart, username,emailAddress, HandleSmtpResetPasswordMessage);
            }
        }
        private void HandleRegistrationRequest_RegistrationEnum(JsonObject jsonObject)
        {
            RegistrationInformation registrationInformation = jsonObject.MessageBody as RegistrationInformation;
            string username = registrationInformation.Username;
            string password = registrationInformation.Password;
            string firstName = registrationInformation.FirstName;
            string lastName = registrationInformation.LastName;
            string emailAddress = registrationInformation.EmailAddress;
            string cityName = registrationInformation.CityName;
            DateTime dateOfBirth = registrationInformation.DateOfBirth;
            DateTime registrationDate = registrationInformation.RegistrationDate;
            string gender = registrationInformation.Gender;
            List<string[]> VerificationQuestionsAndAnswers = registrationInformation.VerificationQuestionsAndAnswers;
            string dateOfBirthAsString = dateOfBirth.ToString("yyyy-MM-dd");
            string registrationDateAsString = registrationDate.ToString("yyyy-MM-dd");

            if (!DataHandler.usernameIsExist(username) /*&& !UserDetails.DataHandler.EmailAddressIsExist(data[4])*/)
            {
                EnumHandler.CommunicationMessageID_Enum RegistrationResponseEnum;
                if (DataHandler.InsertUser(username, password, firstName, lastName, emailAddress, cityName, gender, dateOfBirthAsString, registrationDateAsString, VerificationQuestionsAndAnswers) > 0)
                {
                    _RegistrationFailedAttempts = null;
                    _RegistrationSmtpFailedAttempts = null;

                    _ClientNick = username;
                    RegistrationResponseEnum = EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_SuccessfulRegistration;
                }
                else//if regist not ok
                {
                    RegistrationResponseEnum = EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_FailedRegistration;
                }
                JsonObject RegistrationResponseJsonObject = new JsonObject(RegistrationResponseEnum, null);
                string RegistrationResponseJson = JsonConvert.SerializeObject(RegistrationResponseJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(RegistrationResponseJson);
            }
            else//if regist not ok
            {

                if (_RegistrationFailedAttempts == null)
                {
                    _RegistrationFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Registration);
                }
                HandleFailedAttempt(_RegistrationFailedAttempts, EnumHandler.CommunicationMessageID_Enum.RegistrationBanStart, SendFailedRegistration);
            }
        }
        private void SendFailedRegistration()
        {
            JsonObject RegistrationResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_FailedRegistration, null);
            string RegistrationResponseJson = JsonConvert.SerializeObject(RegistrationResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(RegistrationResponseJson);
        }

        private void HandleRegistrationRequest_UploadProfilePictureRequest(JsonObject jsonObject) //todo - handle the else statment...
        {
            string profilePictureId = jsonObject.MessageBody as string;
            if (UserDetails.DataHandler.InsertProfilePicture(_ClientNick, profilePictureId) > 0)
            {
                JsonObject UploadProfilePictureResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UploadProfilePictureResponse, profilePictureId);
                string UploadProfilePictureResponseJson = JsonConvert.SerializeObject(UploadProfilePictureResponseJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(UploadProfilePictureResponseJson);
            }
        }
        private void HandleRegistrationRequest_UploadStatusRequest(JsonObject jsonObject)
        {
            string status = jsonObject.MessageBody as string;
            if (UserDetails.DataHandler.InsertStatus(_ClientNick, status) > 0)
            {
                JsonObject UploadStatusResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UploadStatusResponse, status);
                string UploadStatusResponseJson = JsonConvert.SerializeObject(UploadStatusResponseJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(UploadStatusResponseJson);
            }
        }
        private void SendFailedLoginMessage()
        {
            JsonObject failedLoginJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.loginResponse_FailedLogin,null);
            string failedLoginJson = JsonConvert.SerializeObject(failedLoginJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(failedLoginJson);
        }
        private void HandleLoginRequest_SmtpLoginCode(JsonObject jsonObject)
        {
            string code = jsonObject.MessageBody as string;

            if (smtpHandler.GetSmtpCode() == code)
            {
                Image catpchaBitmap = captchaCodeHandler.CreateCatpchaBitmap();
                byte[] bytes = ConvertHandler.ConvertImageToBytes(catpchaBitmap);
                ImageContent imageContent = new ImageContent(bytes);
                //ClientAttemptsState clientAttemptsState = null;
                //InitializeClientAttemptsStateObject(ref clientAttemptsState);
                _LoginFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Login);
                JsonObject smtpLoginCodeResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.LoginResponse_SuccessfulSmtpLoginCode, imageContent);
                string smtpLoginCodeResponseJson = JsonConvert.SerializeObject(smtpLoginCodeResponseJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(smtpLoginCodeResponseJson);
            }    
            else
            {
                //ClientAttemptsState clientAttemptsState = GetClientAttemptsStateObject();
                //HandleFailedAttempt(clientAttemptsState, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SendFailedLoginSmtpCode);
                HandleFailedAttempt(_LoginFailedAttempts, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SendFailedLoginSmtpCode);
            }
        }
        private void SendFailedLoginSmtpCode()
        {
            JsonObject smtpLoginCodeResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.LoginResponse_FailedSmtpLoginCode, null);
            string smtpLoginCodeResponseJson = JsonConvert.SerializeObject(smtpLoginCodeResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(smtpLoginCodeResponseJson);
        }
        private void HandleCAptchaBitMap()
        {

            Image catpchaBitmap = captchaCodeHandler.CreateCatpchaBitmap();
            byte[] bytes = ConvertHandler.ConvertImageToBytes(catpchaBitmap);
            ImageContent captchaBitmapContent = new ImageContent(bytes);
            JsonObject smtpLoginCodeResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.CaptchaImageResponse, captchaBitmapContent);;
            string smtpLoginCodeResponseJson = JsonConvert.SerializeObject(smtpLoginCodeResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(smtpLoginCodeResponseJson);
        }

        private void HandleCaptchaImageRequestEnum(JsonObject jsonObject)
        {
            bool afterFail = (bool)jsonObject.MessageBody;
            if (afterFail)
            {
                HandleCaptchaImage();
            }
            else
            {
                HandleFailedAttempt(_LoginFailedAttempts, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, HandleCaptchaImage);
            }
        }
        private void HandleCaptchaImage()
        {
            Image catpchaBitmap = captchaCodeHandler.CreateCatpchaBitmap();
            byte[] bytes = ConvertHandler.ConvertImageToBytes(catpchaBitmap);
            ImageContent captchaBitmapContent = new ImageContent(bytes);
            JsonObject captchaBitmapContentJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.CaptchaImageResponse, captchaBitmapContent); ;
            string captchaBitmapContentJson = JsonConvert.SerializeObject(captchaBitmapContentJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(captchaBitmapContentJson);
        }
        private void HandleDisconnectEnum()
        {
            if (_ClientNick != null)
            {
                DateTime dateTime = DateTime.Now.AddYears(-1);
                DateTime currentDateTime = DataHandler.SetUserOffline(_ClientNick, dateTime);
                if (currentDateTime.CompareTo(dateTime)  > 0)
                {
                    //was ok...
                    _isOnline = false;
                    OfflineDetails offlineDetails = new OfflineDetails(_ClientNick, currentDateTime);
                    List<string> friendNames = DataHandler.GetFriendList(_ClientNick);
                    
                    JsonObject offlineUpdateJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.OfflineUpdate, offlineDetails);
                    string offlineUpdateJson = JsonConvert.SerializeObject(offlineUpdateJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    foreach (string friendName in friendNames)
                    {
                        Unicast(offlineUpdateJson, friendName);
                    }
                }
            }
            NetworkStream stream = _client.GetStream();
            stream.Close();
            _client.Close();
            _client.Dispose();
            _client = null;
            isClientConnected = false;
            AllClients.Remove(_clientIP);
            Logger.LogUserLogOut("A user has logged out from the server.");
        }
        private void HandleCaptchaCodeRequestEnum(JsonObject jsonObject)
        {
            string code = jsonObject.MessageBody as string;

            if (captchaCodeHandler.CompareCode(code))
            {
                CaptchaRotationImageDetails captchaRotationImageDetails = GetCaptchaRotationImageDetails();
                //ClientAttemptsState clientAttemptsState = null;
                //InitializeClientAttemptsStateObject(ref clientAttemptsState);
                _LoginFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Login);

                JsonObject captchaCodeResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.SuccessfulCaptchaCodeResponse, captchaRotationImageDetails);
                string captchaCodeResponseJson = JsonConvert.SerializeObject(captchaCodeResponseJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(captchaCodeResponseJson);
            }
            else
            {
                //ClientAttemptsState clientAttemptsState = GetClientAttemptsStateObject();
                //HandleFailedAttempt(clientAttemptsState, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SendFailedCaptchaCode);
                HandleFailedAttempt(_LoginFailedAttempts, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SendFailedCaptchaCode);

            }
        }
        private void SendFailedCaptchaCode()
        {
            JsonObject captchaCodeResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.FailedCaptchaCodeResponse, null);
            string captchaCodeResponseJson = JsonConvert.SerializeObject(captchaCodeResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(captchaCodeResponseJson);
        }
        private void HandleDeleteMessageRequestEnum(JsonObject jsonObject)
        {
            JsonClasses.Message message = jsonObject.MessageBody as JsonClasses.Message;
            string messageSenderName = message.MessageSenderName;
            if (messageSenderName == _ClientNick)
            {
                string chatId = message.ChatId;

                DateTime messageDateTime = message.MessageDateAndTime;
                XmlFileManager xmlFileManager = ChatHandler.ChatHandler.ChatFileManagers[chatId];
                object messageContent = message.MessageContent;

                ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
                string lastMessageContentValue = "";
                string messageType = "";
                string messageContentAsString = "";
                if (messageContent is string textMessageContent)
                {
                    messageType = "Text";
                    messageContentAsString = textMessageContent;
                    lastMessageContentValue = textMessageContent;
                }
                else if (messageContent is ImageContent imageMessageContent)
                {
                    messageType = "Image";
                    byte[] imageMessageContentByteArray = imageMessageContent.ImageBytes;
                    string imageMessageContentString = Convert.ToBase64String(imageMessageContentByteArray);

                    messageContentAsString = imageMessageContentString;
                    lastMessageContentValue = "Image";
                }
                if (messageDateTime.CompareTo(chat.LastMessageTime) == 0 && messageSenderName == chat.LastMessageSenderName)
                {
                    chat.LastMessageContent = "Deleted Message";
                }
                xmlFileManager.EditMessage(messageSenderName, messageType, messageContentAsString, messageDateTime);

                string TableName = "";
                if (chat is DirectChatDetails)
                    TableName = "DirectChats";
                else if (chat is GroupChatDetails)
                    TableName = "GroupChats";
                string messageDateTimeAsString = messageDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                if (DataHandler.AreLastMessageDataIdentical(TableName, chatId, lastMessageContentValue, messageSenderName, messageDateTimeAsString))
                {
                    if (DataHandler.UpdateLastMessageData(TableName, chatId, "Deleted Message", messageSenderName, messageDateTimeAsString) > 0)
                    {

                    }
                }
                JsonObject deleteMessageJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.DeleteMessageResponse, message);
                string deleteMessageJson = JsonConvert.SerializeObject(deleteMessageJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendChatMessage(deleteMessageJson, chatId);
            }        
        }
        private void HandleSendMessageRequestEnum(JsonObject jsonObject)
        {
            JsonClasses.Message message = jsonObject.MessageBody as JsonClasses.Message;
            string messageSenderName = message.MessageSenderName;
            if (messageSenderName == _ClientNick)
            {
                string chatId = message.ChatId;

                DateTime messageDateTime = message.MessageDateAndTime;
                XmlFileManager xmlFileManager = ChatHandler.ChatHandler.ChatFileManagers[chatId];
                object messageContent = message.MessageContent;

                ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
                chat.LastMessageTime = messageDateTime;
                string lastMessageContentValue = "";
                string messageType = "";
                string messageContentAsString = "";
                if (messageContent is string textMessageContent)
                {
                    messageType = "Text";
                    messageContentAsString = textMessageContent;
                    lastMessageContentValue = textMessageContent;
                }
                else if (messageContent is ImageContent imageMessageContent)
                {
                    messageType = "Image";
                    byte[] imageMessageContentByteArray = imageMessageContent.ImageBytes;
                    string imageMessageContentString = Convert.ToBase64String(imageMessageContentByteArray);

                    messageContentAsString = imageMessageContentString;
                    lastMessageContentValue = "Image";
                }
                chat.LastMessageContent = lastMessageContentValue;
                chat.LastMessageSenderName = messageSenderName;
                string TableName = "";
                if (chat is DirectChatDetails)
                    TableName = "DirectChats";
                else if (chat is GroupChatDetails)
                    TableName = "GroupChats";
                string messageDateTimeAsString = messageDateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                if (DataHandler.UpdateLastMessageData(TableName, chatId, lastMessageContentValue, messageSenderName, messageDateTimeAsString) > 0)
                {
                    xmlFileManager.AppendMessage(messageSenderName, messageType, messageContentAsString, messageDateTime);

                    JsonObject messageJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.SendMessageResponse, message);
                    string messageJson = JsonConvert.SerializeObject(messageJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    SendChatMessage(messageJson, chatId);
                }
               
            }      
        }
        private void HandlePasswordUpdateRequestEnum(JsonObject jsonObject)
        {
            PasswordUpdateDetails passwordUpdateDetails = jsonObject.MessageBody as PasswordUpdateDetails;
            string username = passwordUpdateDetails.Username;
            string pastPassword = passwordUpdateDetails.PastPassword;
            string newPassword = passwordUpdateDetails.NewPassword;
            if (_PasswordUpdateFailedAttempts == null)
            {
                _PasswordUpdateFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.PasswordUpdate);
            }
            if (!DataHandler.isMatchingUsernameAndPasswordExist(username, pastPassword)) 
            {
                HandleFailedAttempt(_PasswordUpdateFailedAttempts, EnumHandler.CommunicationMessageID_Enum.PasswordUpdateBanStart, SendUnmatchedDetailsPasswordUpdateResponse);

                //past password not matching..
            }
            else           
            {
                EnumHandler.CommunicationMessageID_Enum successfulPasswordRenewal = EnumHandler.CommunicationMessageID_Enum.SuccessfulPasswordUpdateResponse;
                EnumHandler.CommunicationMessageID_Enum failedPasswordRenewal = EnumHandler.CommunicationMessageID_Enum.FailedPasswordUpdateResponse_PasswordExist;
                EnumHandler.CommunicationMessageID_Enum errorPasswordRenewal = EnumHandler.CommunicationMessageID_Enum.ErrorHandlePasswordUpdateResponse;
                PasswordRenewalOptions passwordRenewalOptions = new PasswordRenewalOptions(successfulPasswordRenewal, failedPasswordRenewal, errorPasswordRenewal);
                HandlePasswordRenewal(username,newPassword, passwordRenewalOptions, _PasswordUpdateFailedAttempts);
            }
        }
        private void SendUnmatchedDetailsPasswordUpdateResponse()
        {
            JsonObject passwordRenewalJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.FailedPasswordUpdateResponse_UnmatchedDetails, null);
            string passwordRenewalJson = JsonConvert.SerializeObject(passwordRenewalJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(passwordRenewalJson);
        }
        private void HandlePasswordRenewalMessageRequestEnum(JsonObject jsonObject)
        {
            LoginDetails passwordRenewalDetails = jsonObject.MessageBody as LoginDetails;
            string username = passwordRenewalDetails.Username;
            string newPassword = passwordRenewalDetails.Password;
            EnumHandler.CommunicationMessageID_Enum successfulPasswordRenewal = EnumHandler.CommunicationMessageID_Enum.SuccessfulRenewalMessageResponse;
            EnumHandler.CommunicationMessageID_Enum failedPasswordRenewal = EnumHandler.CommunicationMessageID_Enum.FailedRenewalMessageResponse;
            EnumHandler.CommunicationMessageID_Enum errorPasswordRenewal = EnumHandler.CommunicationMessageID_Enum.ErrorHandleRenewalMessageResponse;
            PasswordRenewalOptions passwordRenewalOptions = new PasswordRenewalOptions(successfulPasswordRenewal, failedPasswordRenewal, errorPasswordRenewal);
            if (_PasswordResetFailedAttempts == null)
            {
                _PasswordResetFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.PasswordRestart);
            }
            HandlePasswordRenewal(username, newPassword, passwordRenewalOptions, _PasswordResetFailedAttempts);
        }
        private void SendFailedPasswordRenewal(EnumHandler.CommunicationMessageID_Enum failedPasswordRenewalEnum)
        {
            JsonObject passwordRenewalJsonObject = new JsonObject(failedPasswordRenewalEnum, null);
            string passwordRenewalJson = JsonConvert.SerializeObject(passwordRenewalJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(passwordRenewalJson);
        }
        private void HandlePasswordRenewal(string username, string password, PasswordRenewalOptions passwordRenewalOptions, ClientAttemptsState clientAttemptsState)
        {
            if (DataHandler.PasswordIsExist(username, password)) //means the password already been chosen once by the user...
            {
                EnumHandler.CommunicationMessageID_Enum failedPasswordRenewalEnum = passwordRenewalOptions.GetFailedPasswordRenewal();
                EnumHandler.CommunicationMessageID_Enum BanStartEnumType;
                switch (clientAttemptsState.UserAuthenticationState)
                {
                    case EnumHandler.UserAuthentication_Enum.PasswordRestart:
                        BanStartEnumType = EnumHandler.CommunicationMessageID_Enum.ResetPasswordBanStart;
                        break;
                    default: //EnumHandler.UserAuthentication_Enum.PasswordUpdate (two possibles options...
                        BanStartEnumType = EnumHandler.CommunicationMessageID_Enum.PasswordUpdateBanStart;
                        break;
                }
                HandleFailedAttempt(clientAttemptsState, BanStartEnumType, failedPasswordRenewalEnum, SendFailedPasswordRenewal);
            }
            else
            {
                EnumHandler.CommunicationMessageID_Enum passwordRenewalEnumType;
                if (DataHandler.CheckFullPasswordCapacity(username))
                {
                    DataHandler.AddColumnToUserPastPasswords();
                }
                if (DataHandler.SetNewPassword(username, password) > 0)
                {
                    clientAttemptsState = null;
                    passwordRenewalEnumType = passwordRenewalOptions.GetSuccessfulPasswordRenewal();

                }
                else
                {
                    passwordRenewalEnumType = passwordRenewalOptions.GetErrorPasswordRenewal();
                }
                JsonObject passwordRenewalJsonObject = new JsonObject(passwordRenewalEnumType, null);
                string passwordRenewalJson = JsonConvert.SerializeObject(passwordRenewalJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(passwordRenewalJson);
            }
        }

        private void HandleUdpAudioConnectionRequestEnum(JsonObject jsonObject)
        {
            string portAsString = jsonObject.MessageBody as string;
            int port = 0;
            if (portAsString != null)
            {
                if (int.TryParse(portAsString, out port))
                {
                    // Conversion successful, 'port' now contains the integer value
                    Console.WriteLine($"Parsed port: {port}");
                }
                else
                {
                    // Conversion failed
                    Console.WriteLine("Failed to parse port. Invalid format.");
                }
            }
            IPEndPoint iPEndPoint = new IPEndPoint(_clientAddress, port);

            IpEndPointHandler.UpdateEndPoint(AudioUdpHandler.EndPoints, _clientIPEndPoint, iPEndPoint);

            string udpSymmetricKey = RandomStringCreator.RandomString(32);
            string EncryptedSymmerticKey = Rsa.Encrypt(udpSymmetricKey, ClientPublicKey);
            AudioUdpHandler.clientKeys.Add(iPEndPoint, udpSymmetricKey);
            JsonObject udpAudioConnectionRequestJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UdpAudioConnectionResponse, EncryptedSymmerticKey);
            string udpAudioConnectionRequestJson = JsonConvert.SerializeObject(udpAudioConnectionRequestJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(udpAudioConnectionRequestJson);
        }


        
        private void HandleUdpVideoConnectionRequestEnum(JsonObject jsonObject)
        {
            UdpPorts udpPorts = jsonObject.MessageBody as UdpPorts;
            int audioPort = udpPorts.AudioPort;
            int videoPort = udpPorts.VideoPort;
            IPEndPoint audioIPEndPoint = new IPEndPoint(_clientAddress, audioPort);
            IPEndPoint videoIPEndPoint = new IPEndPoint(_clientAddress, videoPort);

            IpEndPointHandler.UpdateEndPoint(AudioUdpHandler.EndPoints, _clientIPEndPoint, audioIPEndPoint);
            IpEndPointHandler.UpdateEndPoint(VideoUdpHandler.EndPoints, _clientIPEndPoint, videoIPEndPoint);

            string udpAudioSymmetricKey = RandomStringCreator.RandomString(32);
            string encryptedAudioSymmerticKey = Rsa.Encrypt(udpAudioSymmetricKey, ClientPublicKey);
            AudioUdpHandler.clientKeys.Add(audioIPEndPoint, udpAudioSymmetricKey);

            string udpVideoSymmetricKey = RandomStringCreator.RandomString(32);
            string encryptedVideoSymmerticKey = Rsa.Encrypt(udpVideoSymmetricKey, ClientPublicKey);
            VideoUdpHandler.clientKeys.Add(videoIPEndPoint, udpVideoSymmetricKey);

            UdpDetails udpDetails = new UdpDetails(encryptedAudioSymmerticKey, encryptedVideoSymmerticKey);
            JsonObject udpVideoConnectionRequestJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UdpVideoConnectionResponse, udpDetails);
            string udpVideoConnectionRequestJson = JsonConvert.SerializeObject(udpVideoConnectionRequestJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(udpVideoConnectionRequestJson);
        }
        private void HandleUpdateProfileStatusRequestEnum(JsonObject jsonObject)
        {
            string status = jsonObject.MessageBody as string;
            if (DataHandler.InsertStatus(_ClientNick, status) > 0)
            {
                StatusUpdate statusUpdate = new StatusUpdate(_ClientNick, status);
                List<string> friendNames = DataHandler.GetFriendList(_ClientNick);
                JsonObject updateProfileStatusResponseSenderJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UpdateProfileStatusResponse_Sender, status);
                string updateProfileStatusResponseSenderJson = JsonConvert.SerializeObject(updateProfileStatusResponseSenderJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(updateProfileStatusResponseSenderJson);
                JsonObject updateProfileStatusResponseRecieverJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UpdateProfileStatusResponse_Reciever, statusUpdate);
                string updateProfileStatusResponseRecieverJson = JsonConvert.SerializeObject(updateProfileStatusResponseRecieverJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                foreach (string friendName in friendNames)
                {
                    Unicast(updateProfileStatusResponseRecieverJson, friendName);
                }
            }
        }
        private void HandleUpdateProfilePictureRequestEnum(JsonObject jsonObject)
        {
            string profilePictureId = jsonObject.MessageBody as string;
            if (DataHandler.InsertProfilePicture(_ClientNick, profilePictureId) > 0)
            {
                ProfilePictureUpdate profilePictureUpdate = new ProfilePictureUpdate(_ClientNick, profilePictureId);
                List<string> friendNames = DataHandler.GetFriendList(_ClientNick);
                List<string> commonChatUsers = ChatHandler.ChatHandler.GetUserAllChatUsernames(_ClientNick);

                List<string> chatUsers_NotContants = new List<string>();
                foreach (string friend in commonChatUsers)
                {
                    if (!friendNames.Contains(friend))
                    {
                        chatUsers_NotContants.Add(friend);
                    }
                }
                JsonObject updateProfilePictureResponseSenderJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UpdateProfilePictureResponse_Sender, profilePictureId);
                string updateProfilePictureResponseSenderJson = JsonConvert.SerializeObject(updateProfilePictureResponseSenderJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(updateProfilePictureResponseSenderJson);
                JsonObject updateProfilePictureResponseContactRecieverJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UpdateProfilePictureResponse_ContactReciever, profilePictureUpdate);
                string updateProfilePictureResponseContactRecieverJson = JsonConvert.SerializeObject(updateProfilePictureResponseContactRecieverJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                foreach (string friendName in friendNames)
                {
                    Unicast(updateProfilePictureResponseContactRecieverJson, friendName);
                }
                JsonObject updateProfilePictureResponseChatRecieverRecieverJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UpdateProfilePictureResponse_ChatUserReciever, profilePictureUpdate);
                string updateProfilePictureResponseChatRecieverRecieverJsonO = JsonConvert.SerializeObject(updateProfilePictureResponseChatRecieverRecieverJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                foreach (string chatUser in chatUsers_NotContants)
                {
                    Unicast(updateProfilePictureResponseChatRecieverRecieverJsonO, chatUser);
                }
            }
        }
        private void HandleUserDetailsRequestEnum(JsonObject jsonObject)
        {
            JsonClasses.UserDetails userDetails = DataHandler.GetUserProfileSettings(_ClientNick);
            JsonObject userDetailsJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UserDetailsResponse, userDetails);
            string userDetailsJson = JsonConvert.SerializeObject(userDetailsJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(userDetailsJson);
        }

        private void HandleInitialProfileSettingsCheckRequestEnum(JsonObject jsonObject)
        {
            EnumHandler.CommunicationMessageID_Enum PersonalVerificationAnswersNextPhaseEnum;
            
            if (!DataHandler.ProfilePictureIsExist(_ClientNick)) 
            {
                PersonalVerificationAnswersNextPhaseEnum = EnumHandler.CommunicationMessageID_Enum.InitialProfileSettingsCheckResponse_SetUserProfilePicture;

            }
            else if (!DataHandler.StatusIsExist(_ClientNick))
            {
                PersonalVerificationAnswersNextPhaseEnum = EnumHandler.CommunicationMessageID_Enum.InitialProfileSettingsCheckResponse_SetUserProfilePicture;

            }
            else if (DataHandler.SetUserOnline(_ClientNick) > 0)
            {
                _isOnline = true;
                PersonalVerificationAnswersNextPhaseEnum = EnumHandler.CommunicationMessageID_Enum.InitialProfileSettingsCheckResponse_OpenChat;
            }
            else
            {
                PersonalVerificationAnswersNextPhaseEnum = EnumHandler.CommunicationMessageID_Enum.InitialProfileSettingsCheckResponse_HandleError;
            }

            JsonObject personalVerificationAnswersResponseJsonObject = new JsonObject(PersonalVerificationAnswersNextPhaseEnum, null);
            string personalVerificationAnswersResponseJson = JsonConvert.SerializeObject(personalVerificationAnswersResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(personalVerificationAnswersResponseJson);
        }
        private void HandlePersonalVerificationAnswersRequestEnum(JsonObject jsonObject)
        {
            PersonalVerificationAnswers personalVerificationAnswers = jsonObject.MessageBody as PersonalVerificationAnswers;
            EnumHandler.CommunicationMessageID_Enum PersonalVerificationAnswersNextPhaseEnum;

            if (DataHandler.CheckUserVerificationInformation(_ClientNick, personalVerificationAnswers))
            {
                //ClientAttemptsState clientAttemptsState = null;
                //InitializeClientAttemptsStateObject(ref clientAttemptsState);
                _LoginFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Login);
                if (PasswordUpdate.IsNeededToUpdatePassword(_ClientNick)) //opens the user the change password mode, he changes the password and if it's possible it automatticly let him enter or he needs to login once again...
                {
                    PersonalVerificationAnswersNextPhaseEnum = EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_UpdatePassword;
                }
                else if (!DataHandler.ProfilePictureIsExist(_ClientNick)) //todo - change this - after doing the captcha i should ask the server for this information
                {
                    PersonalVerificationAnswersNextPhaseEnum = EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_SetUserProfilePicture;

                }
                else if (!DataHandler.StatusIsExist(_ClientNick))
                {
                    PersonalVerificationAnswersNextPhaseEnum = EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_SetUserStatus;

                }
                else if (DataHandler.SetUserOnline(_ClientNick) > 0)
                {
                    _isOnline = true;
                    PersonalVerificationAnswersNextPhaseEnum = EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_OpenChat;
                }
                else
                {
                    //try again
                    PersonalVerificationAnswersNextPhaseEnum = EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_HandleError;
                }
                JsonObject personalVerificationAnswersResponseJsonObject = new JsonObject(PersonalVerificationAnswersNextPhaseEnum, null);
                string personalVerificationAnswersResponseJson = JsonConvert.SerializeObject(personalVerificationAnswersResponseJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(personalVerificationAnswersResponseJson);
            }
            else
            {
                //ClientAttemptsState clientAttemptsState = GetClientAttemptsStateObject();
                //HandleFailedAttempt(clientAttemptsState, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SetFailedPersonalVerificationAnswers);
                HandleFailedAttempt(_LoginFailedAttempts, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SetFailedPersonalVerificationAnswers);

            }
        }
        private void SetFailedPersonalVerificationAnswers()
        {
            JsonObject personalVerificationAnswersResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.FailedPersonalVerificationAnswersResponse, null);
            string personalVerificationAnswersResponseJson = JsonConvert.SerializeObject(personalVerificationAnswersResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(personalVerificationAnswersResponseJson);
        }
        private void HandleCaptchaImageAngleRequestEnum(JsonObject jsonObject)
        {
            double captchaImageAngle = (double)jsonObject.MessageBody;
            captchaRotatingImageHandler.CheckAngle(captchaImageAngle);
            if (captchaRotatingImageHandler.CheckAttempts())
            {
                if (captchaRotatingImageHandler.CheckSuccess())
                {
                    //ClientAttemptsState clientAttemptsState = null;
                    //InitializeClientAttemptsStateObject(ref clientAttemptsState);
                    _LoginFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Login);
                    string[] userVerificationQuestions = DataHandler.GetUserVerificationQuestions(_ClientNick);
                    string userVerificationQuestion1 = userVerificationQuestions[0];
                    string userVerificationQuestion2 = userVerificationQuestions[1];
                    string userVerificationQuestion3 = userVerificationQuestions[2];
                    string userVerificationQuestion4 = userVerificationQuestions[3];
                    string userVerificationQuestion5 = userVerificationQuestions[4];
                    PersonalVerificationQuestions personalVerificationQuestions = new PersonalVerificationQuestions(userVerificationQuestion1, userVerificationQuestion2, userVerificationQuestion3, userVerificationQuestion4, userVerificationQuestion5);
                    int score = captchaRotatingImageHandler.GetScore();
                    int attempts = captchaRotatingImageHandler.GetAttempts();
                    CaptchaRotationSuccessRate captchaRotationSuccessRate = new CaptchaRotationSuccessRate(score, attempts);
                    PersonalVerificationQuestionDetails verificationQuestionDetails = new PersonalVerificationQuestionDetails(personalVerificationQuestions, captchaRotationSuccessRate);
                    JsonObject verificationQuestionDetailsJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.SuccessfulCaptchaImageAngleResponse, verificationQuestionDetails);
                    string verificationQuestionDetailsJson = JsonConvert.SerializeObject(verificationQuestionDetailsJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    SendMessage(verificationQuestionDetailsJson);
                }
                else
                {
                    if (_captchaRotationImagesAttemptsState == null)
                    {
                        _captchaRotationImagesAttemptsState = new ClientCaptchaRotationImagesAttemptsState(this);
                    }
                    int score = captchaRotatingImageHandler.GetScore();
                    _captchaRotationImagesAttemptsState.HandleBan(score);
                    Logger.LogUserLogOut("A user has been blocked from the server.");
                    double banDuration = _captchaRotationImagesAttemptsState.CurrentBanDuration;
                    JsonObject banMessageJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.LoginBanStart, banDuration);
                    string banMessageJson = JsonConvert.SerializeObject(banMessageJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    SendMessage(banMessageJson);
                }
            }
            else
            {
                CaptchaRotationImageDetails captchaRotationImageDetails = GetCaptchaRotationImageDetails();
                JsonObject captchaCodeResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.CaptchaImageAngleResponse, captchaRotationImageDetails);
                string captchaCodeResponseJson = JsonConvert.SerializeObject(captchaCodeResponseJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(captchaCodeResponseJson);
            }
        }
        public CaptchaRotationImageDetails GetCaptchaRotationImageDetails(int score, int attempts)
        {
            CaptchaRotationImages captchaRotationImages = captchaRotatingImageHandler.GetCaptchaRotationImages();
            CaptchaRotationSuccessRate captchaRotationSuccessRate = new CaptchaRotationSuccessRate(score, attempts);
            CaptchaRotationImageDetails captchaRotationImageDetails = new CaptchaRotationImageDetails(captchaRotationImages, captchaRotationSuccessRate);
            return captchaRotationImageDetails;
        }
        public CaptchaRotationImageDetails GetCaptchaRotationImageDetails()
        {
            int score = captchaRotatingImageHandler.GetScore();
            int attempts = captchaRotatingImageHandler.GetAttempts();
            CaptchaRotationImageDetails captchaRotationImageDetails = GetCaptchaRotationImageDetails(score, attempts);
            return captchaRotationImageDetails;
        }

        private void HandlePastFriendRequestsRequestEnum(JsonObject jsonObject)
        {
            List<PastFriendRequest> friendRequests = DataHandler.CheckFriendRequests(_ClientNick);
            if (friendRequests != null)
            {
                string name;
                foreach (PastFriendRequest pastFriendRequest in friendRequests)
                {
                    name = pastFriendRequest.Username;
                    string profilePicture = DataHandler.GetProfilePicture(name);
                    if (profilePicture != "")
                    {
                        pastFriendRequest.ProfilePicture = profilePicture;
                    }
                }
            }
            PastFriendRequests pastFriendRequests = new PastFriendRequests(friendRequests);
            JsonObject pastFriendRequestsJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.PastFriendRequestsResponse, pastFriendRequests);
            string pastFriendRequestsJson = JsonConvert.SerializeObject(pastFriendRequestsJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(pastFriendRequestsJson);
        }
        private void HandleContactInformationRequestEnum(JsonObject jsonObject)
        {
            List<string> friendNames = DataHandler.GetFriendList(_ClientNick);
            Contacts contacts = DataHandler.GetFriendsProfileInformation(friendNames);
  
            DataHandler.PrintTable("Friends");
            JsonObject contactsJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.ContactInformationResponse, contacts);
            string contactsJson = JsonConvert.SerializeObject(contactsJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(contactsJson);


            JsonObject onlineUpdateJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.OnlineUpdate, _ClientNick);
            string onlineUpdateJson = JsonConvert.SerializeObject(onlineUpdateJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            foreach (string friendName in friendNames)
            {
                Unicast(onlineUpdateJson, friendName);
            }
        }
        private void HandleChatInformationRequestEnum(JsonObject jsonObject)
        {
            List<ChatDetails> userChats = ChatHandler.ChatHandler.GetUserChats(_ClientNick);
            Chats chats = new Chats(userChats);
            JsonObject chatsJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.ChatInformationResponse, chats);
            string chatsJson = JsonConvert.SerializeObject(chatsJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(chatsJson);
        }
        private void HandleGroupCreatorRequestEnum(JsonObject jsonObject)
        {
            ChatCreator newChat = jsonObject.MessageBody as ChatCreator;
            string chatName = newChat.ChatName;
            List<string> chatParticipantNames = newChat.ChatParticipants;
            byte[] chatProfilePictrue = newChat.ChatProfilePictureBytes;
            string ChatTagLine = DataHandler.SetTagLine("GroupChats", "DirectChats");
            string xmlFileName = $"GroupChat - {ChatTagLine} - {chatName}";
            XmlFileManager xmlFileManager = new XmlFileManager(xmlFileName, chatParticipantNames, ChatTagLine); //maybe i should create it before i do handledirectchatcreation and if it dont work to delete it...
            string filePath = xmlFileManager.GetFilePath();
            if (DataHandler.CreateGroupChat(newChat, ChatTagLine, filePath) > 0)
            {
                List<ChatParticipant> chatParticipants = DataHandler.GetChatParticipants(chatParticipantNames);
                ChatHandler.ChatDetails chat = new GroupChatDetails(ChatTagLine, null, "", "", chatParticipants, chatName, chatProfilePictrue);
                ChatHandler.ChatHandler.AllChats.Add(ChatTagLine, chat);
                ChatHandler.ChatHandler.ChatFileManagers.Add(ChatTagLine, xmlFileManager);
                //SendMessage(GroupCreatorResponse, "Group was successfully created");
                GroupChatDetails groupChat = (GroupChatDetails)chat;

                JsonObject groupChatJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.GroupCreatorResponse, groupChat);
                string groupChatJson = JsonConvert.SerializeObject(groupChatJsonObject, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                SendMessage(groupChatJson);
                SendChatMessage(groupChatJson, ChatTagLine);
                //ChatMembersCast(GroupCreatorResponse, DecryptedMessageDetails, chatMembers);
                //needs to send this group creation to every logged user...
            }
            else
            {
                xmlFileManager.DeleteFile();
            }
        }
        private void HandleVideoCallRequestEnum(JsonObject jsonObject)
        {
            List<EnumHandler.CommunicationMessageID_Enum> videoCallResponses = new List<EnumHandler.CommunicationMessageID_Enum>
            {
                EnumHandler.CommunicationMessageID_Enum.SuccessfulVideoCallResponse_Sender,
                EnumHandler.CommunicationMessageID_Enum.SuccessfulVideoCallResponse_Reciever,
                EnumHandler.CommunicationMessageID_Enum.FailedVideoCallResponse
            };

            HandleCallRequest(jsonObject, videoCallResponses);
        }
        private void HandleAudioCallRequestEnum(JsonObject jsonObject)
        {
            List<EnumHandler.CommunicationMessageID_Enum> audioCallResponses = new List<EnumHandler.CommunicationMessageID_Enum>
            {
                EnumHandler.CommunicationMessageID_Enum.SuccessfulAudioCallResponse_Sender,
                EnumHandler.CommunicationMessageID_Enum.SuccessfulAudioCallResponse_Reciever,
                EnumHandler.CommunicationMessageID_Enum.FailedAudioCallResponse
            };

            HandleCallRequest(jsonObject, audioCallResponses);
        }
        private void HandleCallRequest(JsonObject jsonObject,List<EnumHandler.CommunicationMessageID_Enum> callResponses)
        {
            if (callResponses == null || callResponses.Count != 3)
            {
                return;
            }
            EnumHandler.CommunicationMessageID_Enum SuccessfulCallResponse_Sender = callResponses[0];
            EnumHandler.CommunicationMessageID_Enum SuccessfulCallResponse_Reciever = callResponses[1];
            EnumHandler.CommunicationMessageID_Enum FailedCallResponse = callResponses[2];

            string chatId = jsonObject.MessageBody as string;
            ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            try
            {
                DirectChatDetails directChat = (DirectChatDetails)chat;
                string friendName = directChat.GetOtherUserName(_ClientNick);
                if (UserIsOnline(friendName) && !isUserInCall(friendName) && !isUserInCall(_ClientNick))
                {
                    JsonObject senderSuccessfulVideoCallResponsJsonObject = new JsonObject(SuccessfulCallResponse_Sender, null);
                    string SenderSuccessfulVideoCallResponsJson = JsonConvert.SerializeObject(senderSuccessfulVideoCallResponsJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    SendMessage(SenderSuccessfulVideoCallResponsJson);

                    JsonObject recieverSuccessfulVideoCallResponsJsonObject = new JsonObject(SuccessfulCallResponse_Reciever, chatId);
                    string recieverSuccessfulVideoCallResponsJson = JsonConvert.SerializeObject(recieverSuccessfulVideoCallResponsJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    Unicast(recieverSuccessfulVideoCallResponsJson, friendName);

                    _inCall = true;
                    SetUserInCall(friendName, true);
                }
                else
                {
                    JsonObject senderSuccessfulVideoCallResponsJsonObject = new JsonObject(FailedCallResponse, null);
                    string SenderSuccessfulVideoCallResponsJson = JsonConvert.SerializeObject(senderSuccessfulVideoCallResponsJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    SendMessage(SenderSuccessfulVideoCallResponsJson);
                }
            }
            catch
            {

            }
        }
        private void HandleVideoCallAcceptanceRequestEnum(JsonObject jsonObject)
        {
            HandleCallAcceptanceRequestEnum(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallAcceptanceResponse, EnumHandler.CommunicationMessageID_Enum.SuccessfulVideoCallResponse_Reciever, true);
        }
        private void HandleAudioCallAcceptanceRequestEnum(JsonObject jsonObject)
        {
            HandleCallAcceptanceRequestEnum(jsonObject, EnumHandler.CommunicationMessageID_Enum.AudioCallAcceptanceResponse, EnumHandler.CommunicationMessageID_Enum.SuccessfulAudioCallResponse_Reciever,false);
        }
        private void HandleCallAcceptanceRequestEnum(JsonObject jsonObject, EnumHandler.CommunicationMessageID_Enum callAcceptanceResponse, EnumHandler.CommunicationMessageID_Enum failedResponse, bool HandleVideo)
        {
            string chatId = jsonObject.MessageBody as string;
            ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            try
            {
                DirectChatDetails directChat = (DirectChatDetails)chat;
                string friendName = directChat.GetOtherUserName(_ClientNick);
                if (isUserInCall(friendName) && isUserInCall(_ClientNick))
                {
                    JsonObject callAcceptanceResponseJsonObject = new JsonObject(callAcceptanceResponse, chatId);
                    string callAcceptanceResponseJson = JsonConvert.SerializeObject(callAcceptanceResponseJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    SendMessage(callAcceptanceResponseJson);
                    Unicast(callAcceptanceResponseJson, friendName);
                    IPEndPoint friendEndPoint = GetUserEndPoint(friendName);
                    AudioUdpHandler.EndPoints[_clientIPEndPoint] = friendEndPoint;
                    AudioUdpHandler.EndPoints[friendEndPoint] = _clientIPEndPoint;
                    if (HandleVideo)
                    {
                        VideoUdpHandler.EndPoints[_clientIPEndPoint] = friendEndPoint;
                        VideoUdpHandler.EndPoints[friendEndPoint] = _clientIPEndPoint;
                    }
                }
            }
            catch
            {
                RestartCallInvitation(chatId, failedResponse);
            }
        }
        private void HandleVideoCallDenialRequestEnum(JsonObject jsonObject)
        {
            HandleCallDenialRequestEnum(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallDenialResponse, EnumHandler.CommunicationMessageID_Enum.SuccessfulVideoCallResponse_Reciever);
        }
        private void HandleAudioCallDenialRequestEnum(JsonObject jsonObject)
        {
            HandleCallDenialRequestEnum(jsonObject, EnumHandler.CommunicationMessageID_Enum.AudioCallDenialResponse,EnumHandler.CommunicationMessageID_Enum.SuccessfulAudioCallResponse_Reciever);
        }
        private void HandleCallDenialRequestEnum(JsonObject jsonObject, EnumHandler.CommunicationMessageID_Enum callDenialResponse, EnumHandler.CommunicationMessageID_Enum failedResponse)
        {
            string chatId = jsonObject.MessageBody as string;
            ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            try
            {
                DirectChatDetails directChat = (DirectChatDetails)chat;
                string friendName = directChat.GetOtherUserName(_ClientNick);
                if (isUserInCall(friendName) && isUserInCall(_ClientNick))
                {
                    JsonObject callDenialResponseJsonObject = new JsonObject(callDenialResponse, null);
                    string callDenialResponseJson = JsonConvert.SerializeObject(callDenialResponseJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    Unicast(callDenialResponseJson, friendName);

                    _inCall = false;
                    SetUserInCall(friendName, false);
                }
            }
            catch
            {
                RestartCallInvitation(chatId, failedResponse);
            }
        }
        private void HandleMessageHistoryRequestEnum(JsonObject jsonObject)
        {
            string chatId = jsonObject.MessageBody as string;
            XmlFileManager xmlFileManager = ChatHandler.ChatHandler.ChatFileManagers[chatId];
            List<MessageData> messageDatas =  xmlFileManager.ReadChatXml();
            List<JsonClasses.Message> messages = new List<JsonClasses.Message>();
            string messageSenderName;
            object messageContent = null;
            DateTime messageDateAndTime;
            foreach (MessageData messageData in messageDatas)
            {
                if (DateTime.TryParse(messageData.Date, out DateTime dateTime))
                {
                    messageDateAndTime = dateTime;
                }
                else
                {
                    throw new ArgumentException("Invalid date format.");
                }
                switch (messageData.Type)
                {
                    case "Text":
                        messageContent = messageData.Content;
                        break;
                    case "Image":
                        if (!string.IsNullOrEmpty(messageData.Content))
                        {
                            // Convert string to byte array
                            byte[] imageData = Convert.FromBase64String(messageData.Content);
                            ImageContent imageContent = new ImageContent(imageData);
                            messageContent = imageContent;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid content type for Image.");
                        }
                        break;
                    case "DeletedMessage":
                        messageContent = null;
                        break;
                }
                messageSenderName = messageData.Sender;
                JsonClasses.Message message = new JsonClasses.Message(messageSenderName, chatId, messageContent, messageDateAndTime);
                messages.Add(message);
            }
            MessageHistory messageHistory = new MessageHistory(messages);
            JsonObject messageHistoryResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.MessageHistoryResponse, messageHistory);
            string messageHistoryResponseJson = JsonConvert.SerializeObject(messageHistoryResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(messageHistoryResponseJson);
        }
        private void HandleVideoCallMuteRequestEnum(JsonObject jsonObject)
        {
            HandleVideoCallRequest(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallMuteResponse);
        }
        private void HandleVideoCallUnmuteRequestEnum(JsonObject jsonObject)
        {
            HandleVideoCallRequest(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallUnmuteResponse);
        }
        private void HandleVideoCallCameraOnRequestEnum(JsonObject jsonObject)
        {   
            HandleVideoCallRequest(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallCameraOnResponse);
        }
        private void HandleVideoCallCameraOffRequestEnum(JsonObject jsonObject)
        {
            HandleVideoCallRequest(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallCameraOffResponse);
        }
        private void HandleEndVideoCallRequestEnum(JsonObject jsonObject)
        {


            VideoCallOverDetails videoCallOverDetails = jsonObject.MessageBody as VideoCallOverDetails;
            string chatId = videoCallOverDetails.ChatId;
            int audioPort = videoCallOverDetails.AudioSocketPort;
            int videoPort = videoCallOverDetails.VideoSocketPort;

            ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            try
            {
                DirectChatDetails directChat = (DirectChatDetails)chat;
                string friendName = directChat.GetOtherUserName(_ClientNick);
                if (isUserInCall(friendName) && isUserInCall(_ClientNick))
                {
                    JsonObject recieverVideoCallOverResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.EndVideoCallResponse_Reciever, null);
                    string recieverVideoCallOverResponseJson = JsonConvert.SerializeObject(recieverVideoCallOverResponseJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    Unicast(recieverVideoCallOverResponseJson, friendName);
                    JsonObject senderVideoCallOverResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.EndVideoCallResponse_Sender, null);
                    string senderVideoCallOverResponseJson = JsonConvert.SerializeObject(senderVideoCallOverResponseJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    SendMessage(senderVideoCallOverResponseJson);
                    _inCall = false;
                    SetUserInCall(friendName, false);


                    IPEndPoint audioIPEndPoint = new IPEndPoint(_clientAddress, audioPort);
                    IpEndPointHandler.RemoveEndpoints(audioIPEndPoint, AudioUdpHandler.EndPoints, AudioUdpHandler.clientKeys);

                    IPEndPoint videoIPEndPoint = new IPEndPoint(_clientAddress, videoPort);
                    IpEndPointHandler.RemoveEndpoints(videoIPEndPoint, VideoUdpHandler.EndPoints, VideoUdpHandler.clientKeys);

               
                }
            }
            catch
            {
            }
        }
        
        private void HandleEndAudioCallRequestEnum(JsonObject jsonObject)
        {
            AudioCallOverDetails callOverDetails = jsonObject.MessageBody as AudioCallOverDetails;
            string chatId = callOverDetails.ChatId;
            int port = callOverDetails.SocketPort;
            ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            try
            {
                DirectChatDetails directChat = (DirectChatDetails)chat;
                string friendName = directChat.GetOtherUserName(_ClientNick);
                if (isUserInCall(friendName) && isUserInCall(_ClientNick))
                {
                    JsonObject recieverAudioCallOverResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.EndAudioCallResponse_Reciever, null);
                    string recieverAudioCallOverResponseJson = JsonConvert.SerializeObject(recieverAudioCallOverResponseJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    Unicast(recieverAudioCallOverResponseJson, friendName);
                    JsonObject senderAudioCallOverResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.EndAudioCallResponse_Sender, null);
                    string senderAudioCallOverResponseJson = JsonConvert.SerializeObject(senderAudioCallOverResponseJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    SendMessage(senderAudioCallOverResponseJson);
                    _inCall = false;
                    SetUserInCall(friendName, false);

                    IPEndPoint audioIPEndPoint = new IPEndPoint(_clientAddress, port);
                    IpEndPointHandler.RemoveEndpoints(audioIPEndPoint, AudioUdpHandler.EndPoints, AudioUdpHandler.clientKeys);
                }
            }
            catch
            {
            }

        }
      
        private void HandleVideoCallRequest(JsonObject jsonObject, EnumHandler.CommunicationMessageID_Enum videoCallRequest)
        {
            string chatId = jsonObject.MessageBody as string;
            ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            try
            {
                DirectChatDetails directChat = (DirectChatDetails)chat;
                string friendName = directChat.GetOtherUserName(_ClientNick);
                if (isUserInCall(friendName) && isUserInCall(_ClientNick))
                {
                    JsonObject videoCallMuteResponseJsonObject = new JsonObject(videoCallRequest, null);
                    string videoCallMuteResponseJson = JsonConvert.SerializeObject(videoCallMuteResponseJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    Unicast(videoCallMuteResponseJson, friendName);
                }
            }
            catch
            {
            }
        }
        private void RestartCallInvitation(string chatId, EnumHandler.CommunicationMessageID_Enum restartCallInvitation)
        {
            JsonObject senderSuccessfulCallResponsJsonObject = new JsonObject(restartCallInvitation, chatId);
            string senderSuccessfulCallResponsJson = JsonConvert.SerializeObject(senderSuccessfulCallResponsJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(senderSuccessfulCallResponsJson);
        }


        private void ReceiveMessageLength(IAsyncResult ar)
        {
            if (_client != null)
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
                        HandleDisconnectEnum();
                        return;
                    }
                    else // client still connected
                    {
                        byte[] buffer = new byte[bytesRead];
                        Array.Copy(data, buffer, bytesRead);

                        bytesRead = BitConverter.ToInt32(buffer, 0);
                        lock (_client.GetStream())
                        {
                            // continue reading form the client
                            _client.GetStream().BeginRead(data, 0, bytesRead, ReceiveMessage, null);

                        }
                    }
                }
                catch (Exception ex)
                {
                    AllClients.Remove(_clientIP);
                    Logger.LogUserLogOut("A user has logged out from the server.");
                }
            }  
        }
        public void ReceiveMessage(IAsyncResult ar)
        {
            if (_client != null)
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
                        HandleDisconnectEnum();
                        return;
                    }
                    else // client still connected
                    {
                        byte[] buffer = new byte[4];
                        Array.Copy(data, 0, buffer, 0, 4);

                        int value = BitConverter.ToInt32(buffer, 0);
                        int newLength = dataHistory.Length + bytesRead - 4;

                        // Create a new array to hold the combined data
                        byte[] newDataHistory = new byte[newLength];

                        // Copy the existing dataHistory to the new array
                        Array.Copy(dataHistory, 0, newDataHistory, 0, dataHistory.Length);

                        // Copy the MessageData to the end of the new array
                        Array.Copy(data, 4, newDataHistory, dataHistory.Length, bytesRead - 4);

                        // Assign the new array to dataHistory
                        dataHistory = newDataHistory;

                        if (value == 1)
                        {
                            string messageReceived = System.Text.Encoding.ASCII.GetString(dataHistory, 0, dataHistory.Length);
                            byte receivedByteSignal = (byte)messageReceived[0];
                            string actualMessage = messageReceived.Substring(1);
                            // if the client is sending send me datatable
                            if (receivedByteSignal == 1)
                            {
                                actualMessage = Encryption.Encryption.DecryptData(SymmetricKey, actualMessage);
                            }
                            JsonObject jsonObject = JsonConvert.DeserializeObject<JsonObject>(actualMessage, new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto,
                                Binder = new NamespaceAdjustmentBinder(),
                                Converters = { new EnumConverter<EnumHandler.CommunicationMessageID_Enum>() }
                            });
                            EnumHandler.CommunicationMessageID_Enum messageType = (EnumHandler.CommunicationMessageID_Enum)jsonObject.MessageType;

                            switch (messageType)
                            {
                                case EnumHandler.CommunicationMessageID_Enum.EncryptionClientPublicKeySender:
                                    HandleEncryptionClientPublicKeySenderEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.loginRequest:
                                    HandleLoginRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.FriendRequestSender:
                                    HandleFriendRequestSenderEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.RegistrationRequest_SmtpRegistrationMessage:
                                    HandleRegistrationRequest_SmtpRegistrationMessageEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.RegistrationRequest_SmtpRegistrationCode:
                                    HandleRegistrationRequest_SmtpRegistrationCodeEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.RegistrationRequest_Registration:
                                    HandleRegistrationRequest_RegistrationEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.UploadProfilePictureRequest:
                                    HandleRegistrationRequest_UploadProfilePictureRequest(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.UploadStatusRequest:
                                    HandleRegistrationRequest_UploadStatusRequest(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.LoginRequest_SmtpLoginCode:
                                    HandleLoginRequest_SmtpLoginCode(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.loginRequest_SmtpLoginMessage:
                                    HandleloginRequest_SmtpLoginMessage(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.CaptchaImageRequest:
                                    HandleCaptchaImageRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.CaptchaCodeRequest:
                                    HandleCaptchaCodeRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.CaptchaImageAngleRequest:
                                    HandleCaptchaImageAngleRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.SendMessageRequest:
                                    HandleSendMessageRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.Disconnect:
                                    HandleDisconnectEnum();
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.PersonalVerificationAnswersRequest:
                                    HandlePersonalVerificationAnswersRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.PasswordUpdateRequest:
                                    HandlePasswordUpdateRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.InitialProfileSettingsCheckRequest:
                                    HandleInitialProfileSettingsCheckRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.UserDetailsRequest:
                                    HandleUserDetailsRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.FriendRequestResponseSender:
                                    HandleFriendRequestResponseSenderEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.ResetPasswordRequest:
                                    HandleResetPasswordRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.ResetPasswordRequest_SmtpCode:
                                    HandleResetPasswordRequest_SmtpCodeEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.ResetPasswordRequest_SmtpMessage:
                                    HandleResetPasswordRequest_SmtpMessageEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.PastFriendRequestsRequest:
                                    HandlePastFriendRequestsRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.PasswordRenewalMessageRequest:
                                    HandlePasswordRenewalMessageRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.ContactInformationRequest:
                                    HandleContactInformationRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.ChatInformationRequest:
                                    HandleChatInformationRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.GroupCreatorRequest:
                                    HandleGroupCreatorRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.VideoCallRequest:
                                    HandleVideoCallRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.VideoCallAcceptanceRequest:
                                    HandleVideoCallAcceptanceRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.VideoCallDenialRequest:
                                    HandleVideoCallDenialRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.VideoCallMuteRequest:
                                    HandleVideoCallMuteRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.VideoCallUnmuteRequest:
                                    HandleVideoCallUnmuteRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.VideoCallCameraOnRequest:
                                    HandleVideoCallCameraOnRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.VideoCallCameraOffRequest:
                                    HandleVideoCallCameraOffRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.EndVideoCallRequest:
                                    HandleEndVideoCallRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.MessageHistoryRequest:
                                    HandleMessageHistoryRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.AudioCallRequest:
                                    HandleAudioCallRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.AudioCallAcceptanceRequest:
                                    HandleAudioCallAcceptanceRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.AudioCallDenialRequest:
                                    HandleAudioCallDenialRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.UdpAudioConnectionRequest:
                                    HandleUdpAudioConnectionRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.UdpVideoConnectionRequest:
                                    HandleUdpVideoConnectionRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.EndAudioCallRequest:
                                    HandleEndAudioCallRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.DeleteMessageRequest:
                                    HandleDeleteMessageRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.UpdateProfileStatusRequest:
                                    HandleUpdateProfileStatusRequestEnum(jsonObject);
                                    break;
                                case EnumHandler.CommunicationMessageID_Enum.UpdateProfilePictureRequest:
                                    HandleUpdateProfilePictureRequestEnum(jsonObject);
                                    break;
                            }
                            dataHistory = new byte[0];
                        }
                        if (isClientConnected)
                        {
                            lock (_client.GetStream())
                            {
                                _client.GetStream().BeginRead(data, 0, 4, ReceiveMessageLength, null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Broadcast(_ClientNick + " has left the chat.");
                }
            }
        }//end R

      

        /// <summary>
        /// The SendMessage method sends a message to the connected client
        /// </summary>
        /// <param name = "message" > Represents the message the server sends to the connected client</param>
        //public void SendMessage(string jsonMessage, bool needEncryption = true)
        //{
        //    if (_client != null)
        //    {
        //        try
        //        {
        //            System.Net.Sockets.NetworkStream ns;
        //            // we use lock to present multiple threads from using the networkstream object
        //            // this is likely to occur when the server is connected to multiple clients all of 
        //            // them trying to access to the networkstram at the same time.
        //            lock (_client.GetStream())
        //            {
        //                ns = _client.GetStream();
        //            }

        //            byte signal = needEncryption ? (byte)1 : (byte)0;

        //            if (needEncryption)
        //            {
        //                jsonMessage = Encryption.Encryption.EncryptData(SymmetricKey, jsonMessage);
        //            }
        //            string messageToSend = Encoding.UTF8.GetString(new byte[] { signal }) + jsonMessage;
        //            Console.WriteLine(messageToSend);

        //            // Send data to the client
        //            byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(messageToSend);

        //            // Prefixes 4 Bytes Indicating Message Length
        //            byte[] length = BitConverter.GetBytes(bytesToSend.Length); // the length of the message in byte array
        //            byte[] prefixedBuffer = new byte[bytesToSend.Length + sizeof(int)]; // the maximum size of int number in bytes array

        //            Array.Copy(length, 0, prefixedBuffer, 0, sizeof(int)); // to get a fixed size of the prefix to the message
        //            Array.Copy(bytesToSend, 0, prefixedBuffer, sizeof(int), bytesToSend.Length); // add the prefix to the message
        //            Console.WriteLine(prefixedBuffer.ToString());
        //            // Actually send it

        //            ns.Write(prefixedBuffer, 0, prefixedBuffer.Length);
        //            ns.Flush();
        //            //ns.Write(bytesToSend, 0, bytesToSend.Length);
        //            //ns.Flush();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.ToString());
        //        }
        //    }
        //}//end SendMessage

        public void Unicast(string jsonMessage, string UserID)
        {
            Client client;
            foreach (DictionaryEntry c in AllClients)
            {
                client = ((Client)(c.Value));
                if (client._ClientNick == UserID && client._isOnline) //בעתיד להחליף clientnick במשתנה של userid
                {
                    ((Client)(c.Value)).SendMessage(jsonMessage);
                }
            }
        }
        public void SendChatMessage(string jsonMessage, string chatId)
        {
            ChatHandler.ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            if (chat.UserExist(_ClientNick))
            {
                List<ChatParticipant> chatParticipants = chat.ChatParticipants;
                string chatParticipantName;
                //List<string> chatParticipantNames = new List<string>();
                foreach (ChatParticipant chatParticipant in chatParticipants)
                {
                    chatParticipantName = chatParticipant.Username;
                    if ((chatParticipantName != _ClientNick) && UserIsOnline(chatParticipantName))
                    {
                        Unicast(jsonMessage, chatParticipantName);
                    }
                }
            }       
        }

        public void SendMessage(string jsonMessage, bool needEncryption = true)
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

                byte signal = needEncryption ? (byte)1 : (byte)0;

                if (needEncryption)
                {
                    jsonMessage = Encryption.Encryption.EncryptData(SymmetricKey, jsonMessage);
                }
                byte[] jsonMessageBytes = System.Text.Encoding.UTF8.GetBytes(jsonMessage);

                // Create a new byte array to hold the final message
                byte[] totalBytesToSend = new byte[jsonMessageBytes.Length + 1];

                // Copy the signal byte to the first position in the new array
                totalBytesToSend[0] = signal;

                // Copy the message bytes to the remaining positions in the new array
                Array.Copy(jsonMessageBytes, 0, totalBytesToSend, 1, jsonMessageBytes.Length);

                int bufferSize = _client.ReceiveBufferSize;
                byte[] bytesToSend;
                byte[] buffer;
                byte[] length;
                byte[] prefixedBuffer;
                while (totalBytesToSend.Length > 0)
                {
                    if (totalBytesToSend.Length > bufferSize - 8)
                    {
                        bytesToSend = new byte[bufferSize - 8];
                        Array.Copy(totalBytesToSend, 0, bytesToSend, 0, bufferSize - 8);
                        buffer = BitConverter.GetBytes(0); //indicates it's not the last message.
                    }
                    else
                    {
                        bytesToSend = totalBytesToSend;
                        buffer = BitConverter.GetBytes(1); //indicates it's the last message...
                    }

                    length = BitConverter.GetBytes(bytesToSend.Length + sizeof(int));  // the length of the message in byte array
                    prefixedBuffer = new byte[bytesToSend.Length + (2 * sizeof(int))];

                    Array.Copy(length, 0, prefixedBuffer, 0, sizeof(int));
                    Array.Copy(buffer, 0, prefixedBuffer, sizeof(int), sizeof(int));
                    Array.Copy(bytesToSend, 0, prefixedBuffer, (2 * sizeof(int)), bytesToSend.Length);

                    ns.Write(prefixedBuffer, 0, prefixedBuffer.Length);
                    ns.Flush();

                    if (totalBytesToSend.Length > bufferSize - 8)
                    {
                        byte[] newTotalBytesToSend = new byte[totalBytesToSend.Length - (System.Convert.ToInt32(bufferSize) - 8)];

                        Array.Copy(totalBytesToSend, System.Convert.ToInt32(bufferSize) - 8, newTotalBytesToSend, 0, newTotalBytesToSend.Length); // to get a fixed size of the prefix to the message
                        totalBytesToSend = newTotalBytesToSend;
                    }
                    else
                    {
                        totalBytesToSend = new byte[0];
                    }
                }
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

        /// <summary>
        /// The UserIsConnected method checks if a user with the given username is connected by searching for a matching username in the AllClients dictionary
        /// </summary>
        /// <param name="username">Represents the name of the player who tries to login</param>
        /// <returns>It returns true if the username is already connected. OtherWise, it returns false </returns>
        public Boolean UserIsConnected(string username)
        {
            foreach (DictionaryEntry c in AllClients)
            {
                if (((Client)(c.Value))._ClientNick == username) //לבדוק גם אם הוא online...
                    return true;
            }
            return false;
        }
        public Boolean UserIsOnline(string username)
        {
            Client client;
            foreach (DictionaryEntry c in AllClients)
            {
                client = (Client)(c.Value);
                if (client._ClientNick == username && client._isOnline) //לבדוק גם אם הוא online...
                    return true;
            }
            return false;
        }
        public bool isUserInCall(string username)
        {
            Client client;
            foreach (DictionaryEntry c in AllClients)
            {
                client = (Client)(c.Value);
                if (client._ClientNick == username && client._inCall) //לבדוק גם אם הוא online...
                    return true;
            }
            return false;
        }
        public void SetUserInCall(string username, bool value)
        {
            Client client;
            foreach (DictionaryEntry c in AllClients)
            {
                client = (Client)(c.Value);
                if (client._ClientNick == username) //לבדוק גם אם הוא online...
                    client._inCall = value;
            }
        }
        public IPEndPoint GetUserEndPoint(string username)
        {
            Client client;
            foreach (DictionaryEntry c in AllClients)
            {
                client = (Client)(c.Value);
                if (client._ClientNick == username)
                    return client._clientIPEndPoint;
            }
            return null;
        }

    }
}
