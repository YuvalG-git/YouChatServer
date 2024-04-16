﻿using System;
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
using System.CodeDom;
using YouChatServer.JsonClasses;
using YouChatServer.CaptchaHandler;
using YouChatServer.ClientAttemptsStateHandler;

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
        public static Dictionary<string, Chat> AllChats = new Dictionary<string, Chat>();

        //private Dictionary<string, int> _clientFailedAttempts = new Dictionary<string, int>();
        //private Dictionary<IPEndPoint, ClientAttemptsState> _clientLoginFailedAttempts = new Dictionary<IPEndPoint, ClientAttemptsState>();
        //private Dictionary<IPEndPoint, ClientAttemptsState> _clientRegistrationFailedAttempts = new Dictionary<IPEndPoint, ClientAttemptsState>();
        //private Dictionary<IPEndPoint, ClientAttemptsState> _clientPasswordUpdateFailedAttempts = new Dictionary<IPEndPoint, ClientAttemptsState>();
        //private Dictionary<IPEndPoint, ClientAttemptsState> _clientPasswordResetFailedAttempts = new Dictionary<IPEndPoint, ClientAttemptsState>();

        //private Dictionary<IPEndPoint, ClientCaptchaRotationImagesAttemptsState> _clientCaptchaRotationImagesAttemptsState = new Dictionary<IPEndPoint, ClientCaptchaRotationImagesAttemptsState>();
        private ClientAttemptsState _LoginFailedAttempts;
        private ClientAttemptsState _RegistrationFailedAttempts;
        private ClientAttemptsState _RegistrationSmtpFailedAttempts;

        private ClientAttemptsState _PasswordUpdateFailedAttempts;
        private ClientAttemptsState _PasswordResetFailedAttempts;

        private ClientCaptchaRotationImagesAttemptsState _captchaRotationImagesAttemptsState;
        public static List<Client> clientsList = new List<Client>();


        public static Dictionary<IPEndPoint, string> clientKeys = new Dictionary<IPEndPoint, string>();

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

        private bool isClientConnected;
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



        const string RightSmtpCode = "right";
        const string WrongSmtpCode = "wrong";

        /// <summary>
        /// Represents rather the nickname has been sent
        /// </summary>
        private bool ReceiveNick = true;


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
            //ClientAttemptsState clientAttemptsState = null;
            //InitializeClientAttemptsStateObject(ref clientAttemptsState);

        }
        public CaptchaRotatingImageHandler GetCaptchaRotatingImageHandler()
        {
            return captchaRotatingImageHandler;
        }

        public static string RandomKey(int Length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, Length).Select(s => s[Random.Next(s.Length)]).ToArray());
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
                string emailAddress = DataHandler.GetEmailAddress(_ClientNick);
                if (emailAddress != "")
                {
                    smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.LoginMessage);
                    //ClientAttemptsState clientAttemptsState = null;
                    //InitializeClientAttemptsStateObject(ref clientAttemptsState);
                    _LoginFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Login);
                    JsonObject smtpLoginMessageJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.loginResponse_SmtpLoginMessage, null);
                    string smtpLoginMessageJson = JsonConvert.SerializeObject(smtpLoginMessageJsonObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    SendMessage(smtpLoginMessageJson);
                }
                else //shouldn't get here - emailaddress won't be empty...
                {
                    SendFailedLoginMessage();
                }
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
            string username = jsonObject.MessageBody as string;
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
        private void HandleResetPasswordRequest_SmtpCodeEnum(JsonObject jsonObject)
        {
            string code = jsonObject.MessageBody as string;
            EnumHandler.CommunicationMessageID_Enum SmtpResetPasswordCodeResponseEnum;
            if (smtpHandler.GetSmtpCode() == code)
            {
                SmtpResetPasswordCodeResponseEnum = EnumHandler.CommunicationMessageID_Enum.SuccessfulResetPasswordResponse_SmtpCode;
            }
            else
            {
                SmtpResetPasswordCodeResponseEnum = EnumHandler.CommunicationMessageID_Enum.FailedResetPasswordResponse_SmtpCode;
            }
            JsonObject smtpResetPasswordCodeResponseJsonObject = new JsonObject(SmtpResetPasswordCodeResponseEnum, null);
            string smtpResetPasswordCodeResponseJson = JsonConvert.SerializeObject(smtpResetPasswordCodeResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(smtpResetPasswordCodeResponseJson);
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
                            HandleFriendRequestResponse(FriendRequestReceiverUsername, FriendRequestSenderUsername, FriendRequestResponseSender1);
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
                                if (UserIsConnected(FriendRequestReceiverUsername))
                                {
                                    string profilePicture = UserDetails.DataHandler.GetProfilePicture(FriendRequestSenderUsername);
                                    if (profilePicture != "")
                                    {
                                        FriendRequestControlDetails friendRequestControlDetails = new FriendRequestControlDetails(FriendRequestSenderUsername, profilePicture);
                                        JsonObject friendRequestJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.FriendRequestReciever, friendRequestControlDetails);
                                        string friendRequestJson = JsonConvert.SerializeObject(friendRequestJsonObject, new JsonSerializerSettings
                                        {
                                            TypeNameHandling = TypeNameHandling.Auto
                                        });
                                        Unicast(friendRequestJson, FriendRequestReceiverUsername);
                                         //todo - need to handle in the client side how it will work
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
                if (FriendRequestStatus == FriendRequestResponseSender1)
                {
                    if ((DataHandler.CheckFullFriendsCapacity(FriendRequestSenderUsername)) || (DataHandler.CheckFullFriendsCapacity(FriendRequestReceiverUsername)))
                    {
                        DataHandler.AddColumnToFriends();
                    }
                    if (DataHandler.AddFriend(FriendRequestSenderUsername, FriendRequestReceiverUsername) > 0) //one worked...
                    {
                        if (DataHandler.AddFriend(FriendRequestReceiverUsername, FriendRequestSenderUsername) > 0) //both worked...
                        {
                            JsonObject friendRequestResponseJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.FriendRequestResponseReciever, null);
                            string friendRequestResponseJson = JsonConvert.SerializeObject(friendRequestResponseJsonObject, new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto
                            });
                            Unicast(friendRequestResponseJson, FriendRequestSenderUsername);
                        }
                    }
                    //the user accepted the friend request and i should handle them being friends... both by entering to database and sending them message if they are connected so they will add one another in contacts..
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
        private void HandleRegistrationRequest_SmtpRegistrationMessageEnum(JsonObject jsonObject) 
        {
            SmtpDetails userUsernameAndEmailAddress = jsonObject.MessageBody as SmtpDetails;
            string username = userUsernameAndEmailAddress.Username;
            string emailAddress = userUsernameAndEmailAddress.EmailAddress;
            smtpHandler.SendCodeToUserEmail(username,emailAddress,EnumHandler.SmtpMessageType_Enum.RegistrationMessage);
            JsonObject SentEmailNotificationJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_SmtpRegistrationMessage, null);
            string SentEmailNotificationJson = JsonConvert.SerializeObject(SentEmailNotificationJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(SentEmailNotificationJson);
        }
        private void HandleResetPasswordRequest_SmtpMessageEnum(JsonObject jsonObject)
        {
            SmtpDetails userUsernameAndEmailAddress = jsonObject.MessageBody as SmtpDetails;
            string username = userUsernameAndEmailAddress.Username;
            string emailAddress = userUsernameAndEmailAddress.EmailAddress;
            smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.PasswordRenewalMessage);
            JsonObject SentEmailNotificationJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.ResetPasswordResponse_SmtpMessage, null);
            string SentEmailNotificationJson = JsonConvert.SerializeObject(SentEmailNotificationJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(SentEmailNotificationJson);
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

            if (!UserDetails.DataHandler.usernameIsExist(username) /*&& !UserDetails.DataHandler.EmailAddressIsExist(data[4])*/)
            {
                EnumHandler.CommunicationMessageID_Enum RegistrationResponseEnum;
                if (UserDetails.DataHandler.InsertUser(username,password,firstName,lastName,emailAddress,cityName,gender,dateOfBirthAsString,registrationDateAsString,VerificationQuestionsAndAnswers) > 0)
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
        private void HandleDisconnectEnum(JsonObject jsonObject)
        {
            if (_ClientNick != null)
            {
                if (UserDetails.DataHandler.SetUserOffline(_ClientNick) > 0)
                {
                    //was ok...
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
        private void HandleSendMessageRequestEnum(JsonObject jsonObject)
        {
            JsonClasses.Message message = jsonObject.MessageBody as JsonClasses.Message;
            string messageContent = message.MessageContent;
            Multicast(sendMessageResponse, messageContent);
        }
        private void HandlePasswordUpdateRequestEnum(JsonObject jsonObject)
        {
            PasswordUpdateDetails passwordUpdateDetails = jsonObject.MessageBody as PasswordUpdateDetails;
            string username = passwordUpdateDetails.Username;
            string pastPassword = passwordUpdateDetails.PastPassword;
            string newPassword = passwordUpdateDetails.NewPassword;
            if (!DataHandler.isMatchingUsernameAndPasswordExist(username, pastPassword)) 
            {
                if (_PasswordUpdateFailedAttempts == null)
                {
                    _PasswordUpdateFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.PasswordUpdate);
                }
                HandleFailedAttempt(_PasswordUpdateFailedAttempts, EnumHandler.CommunicationMessageID_Enum.PasswordUpdateBanStart, SendUnmatchedDetailsPasswordUpdateResponse);

                //past password not matching..
            }
            else           
            {
                EnumHandler.CommunicationMessageID_Enum successfulPasswordRenewal = EnumHandler.CommunicationMessageID_Enum.SuccessfulPasswordUpdateResponse;
                EnumHandler.CommunicationMessageID_Enum failedPasswordRenewal = EnumHandler.CommunicationMessageID_Enum.FailedPasswordUpdateResponse_PasswordExist;
                EnumHandler.CommunicationMessageID_Enum errorPasswordRenewal = EnumHandler.CommunicationMessageID_Enum.ErrorHandlePasswordUpdateResponse;
                PasswordRenewalOptions passwordRenewalOptions = new PasswordRenewalOptions(successfulPasswordRenewal, failedPasswordRenewal, errorPasswordRenewal);
                HandlePasswordRenewal(username,newPassword, passwordRenewalOptions);
                _PasswordUpdateFailedAttempts = null;
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
            HandlePasswordRenewal(username, newPassword, passwordRenewalOptions);
        }
        private void HandlePasswordRenewal(string username, string password, PasswordRenewalOptions passwordRenewalOptions)
        {
            EnumHandler.CommunicationMessageID_Enum passwordRenewalEnumType;
            if (UserDetails.DataHandler.PasswordIsExist(username, password)) //means the password already been chosen once by the user...
            {
                passwordRenewalEnumType = passwordRenewalOptions.GetFailedPasswordRenewal();
            }
            else
            {
                if (UserDetails.DataHandler.CheckFullPasswordCapacity(username))
                {
                    UserDetails.DataHandler.AddColumnToUserPastPasswords();
                }
                if (UserDetails.DataHandler.SetNewPassword(username, password) > 0)
                {
                    passwordRenewalEnumType = passwordRenewalOptions.GetSuccessfulPasswordRenewal();
                }
                else
                {
                    passwordRenewalEnumType = passwordRenewalOptions.GetErrorPasswordRenewal();
                }
            }
            JsonObject passwordRenewalJsonObject = new JsonObject(passwordRenewalEnumType, null);
            string passwordRenewalJson = JsonConvert.SerializeObject(passwordRenewalJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(passwordRenewalJson);
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
            clientKeys.Add(iPEndPoint, SymmetricKey);
            JsonObject udpAudioConnectionRequestJsonObject = new JsonObject(EnumHandler.CommunicationMessageID_Enum.UdpAudioConnectionResponse, null);
            string udpAudioConnectionRequestJson = JsonConvert.SerializeObject(udpAudioConnectionRequestJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(udpAudioConnectionRequestJson);
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
                if (IsNeededToUpdatePassword()) //opens the user the change password mode, he changes the password and if it's possible it automatticly let him enter or he needs to login once again...
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
        private void HandleResetPasswordRequestEnum(JsonObject jsonObject)
        {
            SmtpDetails smtpDetails = jsonObject.MessageBody as SmtpDetails;
            string username = smtpDetails.Username;
            string emailAddress = smtpDetails.EmailAddress;
            EnumHandler.CommunicationMessageID_Enum resetPasswordResponse;
            if (DataHandler.IsMatchingUsernameAndEmailAddressExist(username, emailAddress))
            {
                smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.PasswordRenewalMessage);
                resetPasswordResponse = EnumHandler.CommunicationMessageID_Enum.SuccessfulResetPasswordResponse;
            }
            else
            {
                resetPasswordResponse = EnumHandler.CommunicationMessageID_Enum.FailedResetPasswordResponse;
            }
            JsonObject resetPasswordResponseJsonObject = new JsonObject(resetPasswordResponse, null);
            string resetPasswordResponseJson = JsonConvert.SerializeObject(resetPasswordResponseJsonObject, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            SendMessage(resetPasswordResponseJson);
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
                        AllClients.Remove(_clientIP);
                        Logger.LogUserLogOut("A user has logged out from the server.");
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
                        AllClients.Remove(_clientIP);
                        Logger.LogUserLogOut("A user has logged out from the server.");
                        return;
                    }
                    else // client still connected
                    {
                        string messageReceived = System.Text.Encoding.ASCII.GetString(data, 0, bytesRead);
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
                                HandleDisconnectEnum(jsonObject);
                                break;
                            case EnumHandler.CommunicationMessageID_Enum.UdpAudioConnectionRequest:
                                HandleUdpAudioConnectionRequestEnum(jsonObject);
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
                            case EnumHandler.CommunicationMessageID_Enum.ChatSettingsChangeRequest:
                                ChatSettings chatSettings = jsonObject.MessageBody as ChatSettings;
                                byte textSizeProperty = (byte)chatSettings.TextSizeProperty; 
                                short messageGapProperty = (short)chatSettings.MessageGapProperty;
                                bool enterKeyPressedProperty = chatSettings.EnterKeyPressedProperty;
                                if (DataHandler.UpdateChatSettings(_ClientNick,textSizeProperty,messageGapProperty, enterKeyPressedProperty) > 0)
                                {
                                    //to send a message saying it was successful...
                                }
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
                        }
                    }
                    if (isClientConnected)
                    {
                        lock (_client.GetStream())
                        {
                            // continue reading from the client
                            //_client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(_client.ReceiveBufferSize), ReceiveMessage, null);
                            _client.GetStream().BeginRead(data, 0, 4, ReceiveMessageLength, null);

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
        /// The ReceiveMessage method recieves and handles the incomming stream
        /// </summary>
        /// <param name="ar">IAsyncResult Interface</param>
        //public void ReceiveMessage(IAsyncResult ar)
        //    {
        //        int bytesRead;
        //        try
        //        {
        //            lock (_client.GetStream())
        //            {
        //                // call EndRead to handle the end of an async read.
        //                bytesRead = _client.GetStream().EndRead(ar);
        //            }
        //            // if bytesread<1 -> the client disconnected
        //            if (bytesRead < 1)
        //            {
        //                // remove the client from out list of clients
        //                AllClients.Remove(_clientIP);
        //                disconnectedClients.Enqueue(PlayerNum);
        //                return;
        //            }
        //            else // client still connected
        //            {
        //                string messageReceived = System.Text.Encoding.ASCII.GetString(data, 0, bytesRead);
        //                string[] messageToArray = messageReceived.Split('$');
        //                int requestNumber = Convert.ToInt32(messageToArray[0]);
        //                string messageDetails = messageToArray[1];
        //                string DecryptedMessageDetails;
        //                // if the client is sending send me datatable
        //                if (requestNumber == EncryptionClientPublicKeySender)
        //                {
        //                    ClientPublicKey = messageDetails;
        //                    SendMessage(EncryptionServerPublicKeyReciever, Rsa.GetPublicKey());
        //                    //SendMessage(EncryptionServerPublicKeyReciever + "$" + Rsa.GetPublicKey());

        //                    SymmetricKey = RandomStringCreator.RandomString(32);
        //                    string EncryptedSymmerticKey = Rsa.Encrypt(SymmetricKey, ClientPublicKey);
        //                    SendMessage(EncryptionSymmetricKeyReciever, EncryptedSymmerticKey);
        //                    //SendMessage(EncryptionSymmetricKeyReciever + "$" + EncryptedSymmerticKey);

        //                }
        //                else
        //                {
        //                    DecryptedMessageDetails = Encryption.Encryption.DecryptData(SymmetricKey, messageDetails);
        //                    if (requestNumber == registerRequest)
        //                    {
        //                        string[] data = DecryptedMessageDetails.Split('#');
        //                        if (!UserDetails.DataHandler.usernameIsExist(data[0]) /*&& !UserDetails.DataHandler.EmailAddressIsExist(data[4])*/)
        //                        {
        //                            if (UserDetails.DataHandler.InsertUser(DecryptedMessageDetails) > 0)
        //                            {
        //                                _ClientNick = data[0];
        //                                SendMessage(registerResponse, registerResponse1);
        //                                //SendMessage(registerResponse + "$" + registerResponse1);

        //                            }
        //                            else//if regist not ok
        //                            {
        //                                SendMessage(registerResponse, registerResponse2);
        //                                //SendMessage(registerResponse + "$" + registerResponse2);

        //                            }
        //                        }
        //                        else//if regist not ok
        //                        {
        //                            SendMessage(registerResponse, registerResponse2);
        //                            //SendMessage(registerResponse + "$" + registerResponse2);

        //                        }
        //                    }
        //                    else if (requestNumber == loginRequest)
        //                    {
        //                        //string[] data = messageDetails.Split('#');
        //                        string[] data = DecryptedMessageDetails.Split('#');

        //                        //if ((UserDetails.DataHandler.isExist(messageDetails)) && (!UserIsConnected(data[0])))

        //                        if ((UserDetails.DataHandler.isExist(DecryptedMessageDetails)) && (!UserIsConnected(data[0])))
        //                        {
        //                            _ClientNick = data[0];
        //                            string emailAddress = UserDetails.DataHandler.GetEmailAddress(_ClientNick);
        //                            if (emailAddress != "")
        //                            {
        //                                SendMessage(loginResponse, emailAddress);
        //                                //SendMessage(loginResponse + "$" + emailAddress);

        //                            }
        //                            else
        //                            {
        //                                //todo - send a message saying there was a problem
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ClientAttemptsState clientAttemptsState;
        //                            if (!_clientFailedAttempts.ContainsKey(_clientAddress))
        //                            {
        //                                clientAttemptsState = new ClientAttemptsState(this);
        //                                _clientFailedAttempts[_clientAddress] = clientAttemptsState;
        //                            }
        //                            else
        //                            {
        //                                clientAttemptsState = _clientFailedAttempts[_clientAddress];
        //                            }
        //                            clientAttemptsState.HandleFailedAttempt();
        //                            //if (_clientFailedAttempts.ContainsKey(_clientIP))
        //                            //{
        //                            //    _clientFailedAttempts[_clientIP]++;

        //                            //}
        //                            //else
        //                            //{
        //                            //    _clientFailedAttempts[_clientIP] = 1;

        //                            //}
        //                            //if (_clientFailedAttempts[_clientIP] > 5)
        //                            //{
        //                            //    //handle waiting..
        //                            //    // todo - handle block of spamming user and maybe do the following act:
        //                            //    //to send a message to the user saying his account got blocked for 10 minutes because someone tried to enter..
        //                            //}
        //                            SendMessage(loginResponse, loginResponse2);
        //                            //SendMessage(loginResponse + "$" + loginResponse2);

        //                        }
        //                    }
        //                    else if (requestNumber == sendMessageRequest)
        //                    {
        //                        string message = _ClientNick + "#" + DecryptedMessageDetails;
        //                        Multicast(sendMessageResponse, message);
        //                        //Broadcast(sendMessageResponse + "$" + message);

        //                    }
        //                    else if (requestNumber == ContactInformationRequest)
        //                    {
        //                        string ContactsInformation = "Dan" + "^" + "hi" + "^" + "07:50" + "^" + "Male3" + "#" + "Ben" + "^" + "how you doing" + "^" + "17:53" + "^" + "Female3 " + "#" + "Ron" + "^" + "YOO" + "^" + "03:43" + "^" + "Male4"; //בעתיד לקחת מידע מהdatabase
        //                        SendMessage(ContactInformationResponse, ContactsInformation);
        //                        //SendMessage(ContactInformationResponse + "$" + ContactsInformation);

        //                    }
        //                    else if ((requestNumber == UploadProfilePictureRequest))
        //                    {
        //                        if (UserDetails.DataHandler.InsertProfilePicture(_ClientNick, DecryptedMessageDetails) > 0)
        //                        {
        //                            SendMessage(UploadProfilePictureResponse, DecryptedMessageDetails);
        //                            //SendMessage(UploadProfilePictureResponse + "$" + registerResponse1);

        //                        }
        //                    }
        //                    else if ((requestNumber == UploadStatusRequest))
        //                    {
        //                        if (UserDetails.DataHandler.InsertStatus(_ClientNick, DecryptedMessageDetails) > 0)
        //                        {
        //                            SendMessage(UploadStatusResponse, DecryptedMessageDetails);
        //                            //SendMessage(UploadStatusResponse + "$" + registerResponse1);

        //                        }
        //                    }
        //                    else if (requestNumber == ResetPasswordRequest)
        //                    {
        //                        if (UserDetails.DataHandler.IsMatchingUsernameAndEmailAddressExist(DecryptedMessageDetails))
        //                        {
        //                            SendMessage(ResetPasswordResponse, ResetPasswordResponse1);
        //                            //SendMessage(ResetPasswordResponse + "$" + ResetPasswordResponse1);

        //                        }
        //                        else
        //                        {
        //                            SendMessage(ResetPasswordResponse, ResetPasswordResponse2);
        //                            //SendMessage(ResetPasswordResponse + "$" + ResetPasswordResponse2);

        //                        }
        //                    }
        //                    else if ((requestNumber == PasswordRenewalMessageRequest) || (requestNumber == PasswordUpdateRequest))
        //                    {
        //                        bool IsPasswordRenewalMessageRequest = (requestNumber == PasswordRenewalMessageRequest);
        //                        int identifierNumber; //maybe instead of handling both here i should write a method that get the idnumber and send the message accordinglly...
        //                        bool HasAccessToChange = true;
        //                        string[] data = DecryptedMessageDetails.Split('#');
        //                        string username = data[0];
        //                        string NewPassword = data[1];
        //                        if (IsPasswordRenewalMessageRequest)
        //                        {
        //                            identifierNumber = PasswordRenewalMessageResponse;
        //                        }
        //                        else
        //                        {
        //                            identifierNumber = PasswordUpdateResponse;

        //                        }
        //                        if (!IsPasswordRenewalMessageRequest)
        //                        {
        //                            string OldPassword = data[2];
        //                            if (!UserDetails.DataHandler.PasswordIsExist(username, OldPassword)) //means the password already been chosen once by the user...
        //                            {
        //                                HasAccessToChange = false;
        //                                SendMessage(identifierNumber, PasswordMessageResponse4); //past password not matching..

        //                            }
        //                        }
        //                        if (HasAccessToChange)
        //                        {
        //                            if (UserDetails.DataHandler.PasswordIsExist(username, NewPassword)) //means the password already been chosen once by the user...
        //                            {
        //                                SendMessage(identifierNumber, PasswordMessageResponse1);

        //                            }
        //                            else
        //                            {
        //                                if (UserDetails.DataHandler.CheckFullPasswordCapacity(username))
        //                                {
        //                                    UserDetails.DataHandler.AddColumnToUserPastPasswords();
        //                                }
        //                                if (UserDetails.DataHandler.SetNewPassword(username, NewPassword) > 0)
        //                                {
        //                                    SendMessage(identifierNumber, PasswordMessageResponse2);
        //                                    //SendMessage(PasswordRenewalMessageResponse + "$" + PasswordRenewalMessageResponse2);
        //                                }
        //                                else
        //                                {
        //                                    SendMessage(identifierNumber, PasswordMessageResponse3);
        //                                    //SendMessage(PasswordRenewalMessageResponse + "$" + PasswordRenewalMessageResponse3);

        //                                }
        //                            }
        //                        }

        //                    }
        //                    else if (requestNumber == InitialProfileSettingsCheckRequest)
        //                    {
        //                        if (IsNeededToUpdatePassword()) //opens the user the change password mode, he changes the password and if it's possible it automatticly let him enter or he needs to login once again...
        //                        {
        //                            SendMessage(InitialProfileSettingsCheckResponse, InitialProfileSettingsCheckResponse1);

        //                        }
        //                        else if (!UserDetails.DataHandler.ProfilePictureIsExist(_ClientNick)) //todo - change this - after doing the captcha i should ask the server for this information
        //                        {
        //                            SendMessage(InitialProfileSettingsCheckResponse, InitialProfileSettingsCheckResponse2);
        //                            //SendMessage(InitialProfileSettingsCheckResponse + "$" + loginResponse4);

        //                        }
        //                        else if (!UserDetails.DataHandler.StatusIsExist(_ClientNick))
        //                        {
        //                            SendMessage(InitialProfileSettingsCheckResponse, InitialProfileSettingsCheckResponse3);
        //                            //SendMessage(InitialProfileSettingsCheckResponse + "$" + loginResponse5);
        //                        }
        //                        else
        //                        {
        //                            SendMessage(InitialProfileSettingsCheckResponse, InitialProfileSettingsCheckResponse4);
        //                            //SendMessage(InitialProfileSettingsCheckResponse + "$" + loginResponse1);
        //                            if (UserDetails.DataHandler.SetUserOnline(_ClientNick) > 0)
        //                            {
        //                                //was ok...
        //                            }

        //                        }
        //                    }
        //                    else if (requestNumber == UserDetailsRequest)
        //                    {
        //                        string UserInformation = UserDetails.DataHandler.GetUserProfileSettings(_ClientNick);
        //                        SendMessage(UserDetailsResponse, UserInformation);
        //                    }
        //                    else if (requestNumber == FriendRequestSender) //todo - needs to check if the user already sent a friend request before..
        //                    {
        //                        string[] data = DecryptedMessageDetails.Split('#');
        //                        string FriendRequestReceiverUsername = data[0];
        //                        string FriendRequestSenderUsername = _ClientNick;

        //                        if (FriendRequestSenderUsername != FriendRequestReceiverUsername)
        //                        {
        //                            if (UserDetails.DataHandler.IsMatchingUsernameAndTagLineIdExist(DecryptedMessageDetails))
        //                            {
        //                                //ask friend request..
        //                                if (UserDetails.DataHandler.AddFriendRequest(FriendRequestSenderUsername, FriendRequestReceiverUsername) > 0)
        //                                {
        //                                    //was successful
        //                                    //to check if he is online...
        //                                    if (UserIsConnected(FriendRequestReceiverUsername))
        //                                    {
        //                                        string profilePicture = UserDetails.DataHandler.GetProfilePicture(FriendRequestSenderUsername);
        //                                        if (profilePicture != "")
        //                                        {
        //                                            string userDetails = FriendRequestSenderUsername + "^" + profilePicture;
        //                                            Unicast(FriendRequestReceiver, userDetails, FriendRequestReceiverUsername); //todo - need to handle in the client side how it will work
        //                                                                                                                        //need to handle when logging in if there were message request sent before...
        //                                        }

        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    //wasn't successful even though details were right - needs to inform the user and tell him to send once again...
        //                                }

        //                            }
        //                        }

        //                    }
        //                    else if (requestNumber == FriendRequestResponseSender)
        //                    {
        //                        string[] data = DecryptedMessageDetails.Split('#'); //needs to string from the client the name of the user who sent and then the accept/deny...
        //                        string FriendRequestSenderUsername = data[0];
        //                        string FriendRequestReceiverUsername = _ClientNick;
        //                        string FriendRequestStatus = data[1];
        //                        if (UserDetails.DataHandler.HandleFriendRequestStatus(FriendRequestSenderUsername, FriendRequestReceiverUsername, FriendRequestStatus) > 0)
        //                        {
        //                            if (FriendRequestStatus == FriendRequestResponseSender1)
        //                            {
        //                                if ((UserDetails.DataHandler.CheckFullFriendsCapacity(FriendRequestSenderUsername)) || (UserDetails.DataHandler.CheckFullFriendsCapacity(FriendRequestReceiverUsername)))
        //                                {
        //                                    UserDetails.DataHandler.AddColumnToFriends();
        //                                }
        //                                if (UserDetails.DataHandler.AddFriend(FriendRequestSenderUsername, FriendRequestReceiverUsername) > 0) //one worked...
        //                                {
        //                                    if (UserDetails.DataHandler.AddFriend(FriendRequestReceiverUsername, FriendRequestSenderUsername) > 0) //both worked...
        //                                    {
        //                                        Unicast(FriendRequestResponseReceiver, "the friend request has been accepted", FriendRequestSenderUsername);
        //                                    }
        //                                }
        //                                //if (UserDetails.DataHandler.setne(username, password) > 0)
        //                                //{
        //                                //    SendMessage(PasswordRenewalMessageResponse, PasswordRenewalMessageResponse2);
        //                                //    //SendMessage(PasswordRenewalMessageResponse + "$" + PasswordRenewalMessageResponse2);
        //                                //}
        //                                //else
        //                                //{
        //                                //    SendMessage(PasswordRenewalMessageResponse, PasswordRenewalMessageResponse3);
        //                                //    //SendMessage(PasswordRenewalMessageResponse + "$" + PasswordRenewalMessageResponse3);

        //                                //}                                    //the user accepted the friend request and i should handle them being friends... both by entering to database and sending them message if they are connected so they will add one another in contacts..
        //                            }
        //                            else if (FriendRequestStatus == FriendRequestResponseSender2)
        //                            {
        //                                // doesn't really need to do something... maybe in the future i will think abt something
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //was an error...
        //                        }

        //                    }
        //                    else if (requestNumber == FriendsProfileDetailsRequest)
        //                    {
        //                        //string FriendsName = UserDetails.DataHandler.GetFriendList(_ClientNick);
        //                        //Dictionary<string, string> FriendsProfileDetailsDictionary = UserDetails.DataHandler.GetFriendsProfileInformation(FriendsName);
        //                        ////here i check the need for split messages...
        //                        //List<string> FriendsProfileDetails = new List<string>();
        //                        //int AllFriendProfileDetailsLength = 0;
        //                        //string FriendProfileDetails = "";
        //                        //int index = 0;
        //                        //foreach (KeyValuePair<string, string> kvp in FriendsProfileDetailsDictionary)
        //                        //{
        //                        //    FriendProfileDetails = "#" + kvp.Value;
        //                        //    byte[] currentUserDetailsBytes = Encoding.UTF8.GetBytes(FriendProfileDetails);

        //                        //    if (currentUserDetailsBytes.Length + AllFriendProfileDetailsLength + 4> 1500)      //needs to check if adding the content of the profiledetails will be two much length
        //                        //    {
        //                        //        index += 1;
        //                        //        AllFriendProfileDetailsLength = 0;
        //                        //    }
        //                        //    FriendsProfileDetails[index] += FriendProfileDetails;
        //                        //    AllFriendProfileDetailsLength += currentUserDetailsBytes.Length;
        //                        //}
        //                        //foreach (string FriendsProfileDetailsSet in FriendsProfileDetails)
        //                        //{
        //                        //    SendMessage(FriendsProfileDetailsResponse, FriendsProfileDetailsSet.Remove(0, 1)); //maybe i should split to couple of messages...
        //                        //}
        //                    }
        //                    else if (requestNumber == disconnectRequest)
        //                    {
        //                        if (_ClientNick != null)
        //                        {
        //                            if (UserDetails.DataHandler.SetUserOffline(_ClientNick) > 0)
        //                            {
        //                                //was ok...
        //                            }
        //                        }
        //                    }
        //                    else if (requestNumber == PastFriendRequestsRequest)
        //                    {
        //                        string FriendRequestNamesOfSendersAndRequestDates = UserDetails.DataHandler.CheckFriendRequests(_ClientNick);
        //                        string[] FriendRequestDetails = FriendRequestNamesOfSendersAndRequestDates.Split('#');
        //                        string[] SplittedFriendRequestDetails;
        //                        string name;
        //                        string DetailsOfFriendRequestSenders = "";
        //                        //for (int index = FriendRequestDetails.Length - 1; index >= 0; index--)
        //                        //{
        //                        //    SplittedFriendRequestDetails = FriendRequestDetails[index].Split('^');
        //                        //    name = SplittedFriendRequestDetails[0];
        //                        //    string profilePicture = UserDetails.DataHandler.GetProfilePicture(name);
        //                        //    if (profilePicture != "")
        //                        //    {
        //                        //        DetailsOfFriendRequestSenders = FriendRequestDetails[index] + "^" + profilePicture + "#";
        //                        //    }
        //                        //}
        //                        foreach (string friendRequestDetails in FriendRequestDetails)
        //                        {
        //                            SplittedFriendRequestDetails = friendRequestDetails.Split('^');
        //                            name = SplittedFriendRequestDetails[0];
        //                            string profilePicture = UserDetails.DataHandler.GetProfilePicture(name);
        //                            if (profilePicture != "")
        //                            {
        //                                DetailsOfFriendRequestSenders += friendRequestDetails + "^" + profilePicture + "#";
        //                            }
        //                        }
        //                        if (DetailsOfFriendRequestSenders.EndsWith("#"))
        //                        {
        //                            DetailsOfFriendRequestSenders = DetailsOfFriendRequestSenders.Substring(0, DetailsOfFriendRequestSenders.Length - 1);
        //                        }
        //                        SendMessage(PastFriendRequestsResponse, DetailsOfFriendRequestSenders);

        //                    }
        //                    else if (requestNumber == VideoCallRequest)
        //                    {
        //                        string friendName = DecryptedMessageDetails;
        //                        if ((UserIsConnected(friendName)) && (DataHandler.StatusIsExist(friendName))) //to check if he is already in a call...
        //                        {
        //                            //establish a udp connection between them two and the server...
        //                            string messageContent = VideoCallResponse2 + "#" + _ClientNick;
        //                            Unicast(VideoCallResponse, messageContent, friendName); //what if he is currently in a call? will it work //todo - needs to check that in the future
        //                        }
        //                        else
        //                        {
        //                            SendMessage(VideoCallResponse, VideoCallResponse1);

        //                        }
        //                    }
        //                    else if (requestNumber == VideoCallResponseSender)
        //                    {
        //                        string[] messageContent = DecryptedMessageDetails.Split('#');
        //                        string messageInformation = messageContent[0];
        //                        string friendName = messageContent[1];
        //                        if (messageInformation == VideoCallResponseResult1)
        //                        {
        //                            Unicast(VideoCallResponseReciever, VideoCallResponseResult1, friendName);
        //                            //needs to create the udp connection...

        //                            //to do something like that:
        //                            //Guid callId = Guid.NewGuid(); // Generate a unique identifier for the call
        //                            //VideoCallMembers call = new VideoCallMembers(_clientIP,_clientAddress);

        //                            //Program.VideoCalls[callId] = call;

        //                        }
        //                        else //the call wont happen...
        //                        {
        //                            Unicast(VideoCallResponseReciever, VideoCallResponseResult2, friendName);

        //                        }
        //                    }
        //                    else if (requestNumber == GroupCreatorRequest)
        //                    {
        //                        ChatCreator newChat = JsonConvert.DeserializeObject<ChatCreator>(DecryptedMessageDetails);
        //                        if (DataHandler.CreateGroupChat(newChat) > 0)
        //                        {
        //                            SendMessage(GroupCreatorResponse, "Group was successfully created");
        //                            List<string> chatMembers = newChat._chatParticipants;
        //                            chatMembers.RemoveAt(0);
        //                            ChatMembersCast(GroupCreatorResponse, DecryptedMessageDetails, chatMembers);
        //                            //needs to send this group creation to every logged user...
        //                        }

        //                    }
        //                }



        //            }
        //            lock (_client.GetStream())
        //            {
        //                // continue reading from the client
        //                _client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(_client.ReceiveBufferSize), ReceiveMessage, null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            AllClients.Remove(_clientIP);
        //            //Broadcast(_ClientNick + " has left the chat.");

        //        }
        //    }//end ReceiveMessage

        /// <summary>
        /// The SendMessage method sends a message to the connected client
        /// </summary>
        /// <param name = "message" > Represents the message the server sends to the connected client</param>
        public void SendMessage(string jsonMessage, bool needEncryption = true)
        {
            if (_client != null)
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
                    string messageToSend = Encoding.UTF8.GetString(new byte[] { signal }) + jsonMessage;
                    Console.WriteLine(messageToSend);

                    // Send data to the client
                    byte[] bytesToSend = System.Text.Encoding.ASCII.GetBytes(messageToSend);

                    // Prefixes 4 Bytes Indicating Message Length
                    byte[] length = BitConverter.GetBytes(bytesToSend.Length); // the length of the message in byte array
                    byte[] prefixedBuffer = new byte[bytesToSend.Length + sizeof(int)]; // the maximum size of int number in bytes array

                    Array.Copy(length, 0, prefixedBuffer, 0, sizeof(int)); // to get a fixed size of the prefix to the message
                    Array.Copy(bytesToSend, 0, prefixedBuffer, sizeof(int), bytesToSend.Length); // add the prefix to the message

                    // Actually send it

                    ns.Write(prefixedBuffer, 0, prefixedBuffer.Length);
                    ns.Flush();
                    //ns.Write(bytesToSend, 0, bytesToSend.Length);
                    //ns.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }//end SendMessage

        public void Unicast(string jsonMessage, string UserID, bool needEncryption = true)
        {
            foreach (DictionaryEntry c in AllClients)
            {
                if (((Client)(c.Value))._ClientNick == UserID) //בעתיד להחליף clientnick במשתנה של userid
                {
                    ((Client)(c.Value)).SendMessage(jsonMessage, needEncryption);
                }
            }
        }

        //public void SendMessage(string jsonMessage, bool needEncryption = true)
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

        //        byte signal = needEncryption ? (byte)1 : (byte)0;

        //        if (needEncryption)
        //        {
        //            jsonMessage = Encryption.Encryption.EncryptData(SymmetricKey, jsonMessage);
        //        }
        //        string messageToSend = Encoding.UTF8.GetString(new byte[] { signal }) + jsonMessage;
        //        Console.WriteLine(messageToSend);

        //        // Send data to the client
        //        byte[] totalBytesToSend = System.Text.Encoding.ASCII.GetBytes(messageToSend);
        //        byte[] bytesToSend;

        //        while (totalBytesToSend.Length > 0)
        //        {
        //            if (totalBytesToSend.Length > System.Convert.ToInt32(_client.ReceiveBufferSize) - 4)
        //            {
        //                bytesToSend = new byte[System.Convert.ToInt32(_client.ReceiveBufferSize) - 4];
        //                Array.Copy(totalBytesToSend, 0, bytesToSend, 0, System.Convert.ToInt32(_client.ReceiveBufferSize) - 4); // to get a fixed size of the prefix to the message
        //                byte[] buffer = BitConverter.GetBytes(0); //indicates it's not the last message...

        //                byte[] length = BitConverter.GetBytes(bytesToSend.Length); // the length of the message in byte array
        //                byte[] prefixedBuffer = new byte[bytesToSend.Length + (2 * sizeof(int))]; // the maximum size of int number in bytes array

        //                Array.Copy(length, 0, prefixedBuffer, 0, sizeof(int)); // to get a fixed size of the prefix to the message
        //                Array.Copy(buffer, 0, prefixedBuffer, sizeof(int), sizeof(int)); // to get a fixed size of the prefix to the message

        //                Array.Copy(bytesToSend, 0, prefixedBuffer, (2 * sizeof(int)), bytesToSend.Length); // add the prefix to the message

        //                // Actually send it

        //                ns.Write(prefixedBuffer, 0, prefixedBuffer.Length);
        //                ns.Flush();
        //                byte[] newTotalBytesToSend = new byte[totalBytesToSend.Length - (System.Convert.ToInt32(_client.ReceiveBufferSize) - 4)];

        //                Array.Copy(totalBytesToSend, System.Convert.ToInt32(_client.ReceiveBufferSize) - 4, newTotalBytesToSend, 0, newTotalBytesToSend.Length); // to get a fixed size of the prefix to the message
        //                totalBytesToSend = newTotalBytesToSend;
        //            }
        //            else
        //            {
        //                byte[] buffer = BitConverter.GetBytes(1); //indicates it's the last message...

        //                byte[] length = BitConverter.GetBytes(totalBytesToSend.Length); // the length of the message in byte array
        //                byte[] prefixedBuffer = new byte[totalBytesToSend.Length + (2 * sizeof(int))]; // the maximum size of int number in bytes array

        //                Array.Copy(length, 0, prefixedBuffer, 0, sizeof(int)); // to get a fixed size of the prefix to the message
        //                Array.Copy(buffer, 0, prefixedBuffer, sizeof(int), sizeof(int)); // to get a fixed size of the prefix to the message

        //                Array.Copy(totalBytesToSend, 0, prefixedBuffer, (2 * sizeof(int)), totalBytesToSend.Length); // add the prefix to the message

        //                // Actually send it

        //                ns.Write(prefixedBuffer, 0, prefixedBuffer.Length);
        //                ns.Flush();
        //                totalBytesToSend = new byte[0];
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}//end SendMessage

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
                if (((Client)(c.Value))._ClientNick == username) //לבדוק גם אם הוא online...
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
