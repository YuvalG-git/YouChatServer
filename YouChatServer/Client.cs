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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace YouChatServer
{
    /// <summary>
    /// The "Client" class is responsible for the client communication.
    /// </summary>
    class Client
    {
        #region Private Fields

        /// <summary>
        /// The ClientAttemptsState object '_LoginFailedAttempts' represents the state of failed login attempts for a client.
        /// </summary>
        private ClientAttemptsState _LoginFailedAttempts;

        /// <summary>
        /// The ClientAttemptsState object '_RegistrationFailedAttempts' represents the state of failed registration attempts for a client.
        /// </summary>
        private ClientAttemptsState _RegistrationFailedAttempts;

        /// <summary>
        /// The ClientAttemptsState object '_RegistrationSmtpFailedAttempts' represents the state of failed SMTP registration attempts for a client.
        /// </summary>
        private ClientAttemptsState _RegistrationSmtpFailedAttempts;

        /// <summary>
        /// The ClientAttemptsState object '_PasswordUpdateFailedAttempts' represents the state of failed password update attempts for a client.
        /// </summary>
        private ClientAttemptsState _PasswordUpdateFailedAttempts;

        /// <summary>
        /// The ClientAttemptsState object '_PasswordResetFailedAttempts' represents the state of failed password reset attempts for a client.
        /// </summary>
        private ClientAttemptsState _PasswordResetFailedAttempts;

        /// <summary>
        /// The ClientCaptchaRotationImagesAttemptsState object '_captchaRotationImagesAttemptsState' represents the state of captcha rotation image attempts for a client.
        /// </summary>
        private ClientCaptchaRotationImagesAttemptsState _captchaRotationImagesAttemptsState;

        /// <summary>
        /// The TcpClient object '_client' represents the client connection.
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// The string '_clientIP' represents the IP address of the client.
        /// </summary>
        private string _clientIP;

        /// <summary>
        /// The IPAddress object '_clientAddress' represents the IP address of the client.
        /// </summary>
        private IPAddress _clientAddress;

        /// <summary>
        /// The IPEndPoint object '_clientIPEndPoint' represents the IP endpoint of the client.
        /// </summary>
        private IPEndPoint _clientIPEndPoint;

        /// <summary>
        /// A byte array 'dataHistory' representing the data received from the client.
        /// </summary>
        private byte[] dataHistory;

        /// <summary>
        /// The string '_ClientNick' represents the nickname of the client.
        /// </summary>
        private string _ClientNick;

        /// <summary>
        /// A boolean value indicating whether the client is online.
        /// </summary>
        private bool _isOnline;

        /// <summary>
        /// A boolean value indicating whether the client is in a call.
        /// </summary>
        private bool _inCall;

        /// <summary>
        /// Byte array representing the data received from the client, used for both sending and receiving data.
        /// </summary>
        private byte[] data;

        /// <summary>
        /// A boolean value indicating whether the client is connected.
        /// </summary>
        private bool _isClientConnected;

        /// <summary>
        /// The RSAServiceProvider object 'Rsa' represents the RSA encryption service provider.
        /// </summary>
        private RSAServiceProvider Rsa;

        /// <summary>
        /// The string 'ClientPublicKey' represents the client's public RSA key.
        /// </summary>
        private string ClientPublicKey;

        /// <summary>
        /// The string 'SymmetricKey' represents the symmetric encryption key.
        /// </summary>
        private string SymmetricKey;

        /// <summary>
        /// The SmtpHandler object 'smtpHandler' represents the SMTP handler used for email communications.
        /// </summary>
        private SmtpHandler smtpHandler;

        /// <summary>
        /// The EncryptionExpirationDate object 'encryptionExpirationDate' represents the expiration date for encryption.
        /// </summary>
        private EncryptionExpirationDate encryptionExpirationDate;

        /// <summary>
        /// The CaptchaCodeHandler object 'captchaCodeHandler' represents the captcha code handler.
        /// </summary>
        private CaptchaCodeHandler captchaCodeHandler;

        /// <summary>
        /// The CaptchaRotatingImageHandler object 'captchaRotatingImageHandler' represents the captcha rotating image handler.
        /// </summary>
        private CaptchaRotatingImageHandler captchaRotatingImageHandler;

        #endregion

        #region Private Const Fields

        /// <summary>
        /// A constant string 'ApprovalFriendRequestResponse' representing the response for approving a friend request.
        /// </summary>
        private const string ApprovalFriendRequestResponse = "Approval";

        /// <summary>
        /// A constant string 'RejectionFriendRequestResponse' representing the response for rejecting a friend request.
        /// </summary>
        private const string RejectionFriendRequestResponse = "Rejection";

        #endregion

        #region Public Static Fields

        /// <summary>
        /// A Hashtable containing all clients in the system.
        /// </summary>
        public static Hashtable AllClients = new Hashtable();

        #endregion

        #region Constructors

        /// <summary>
        /// The "Client" constructor initializes a new instance of the <see cref="Client"/> class with the specified TCP client.
        /// </summary>
        /// <param name="client">The TCP client associated with the client.</param>
        /// <remarks>
        /// This constructor handles the initialization of a new client connection to the server.
        /// It logs the user's login, retrieves the client's IP address for registration, adds the client to the server's client list,
        /// sets up async data reading, and initializes various handlers and state objects for the client.
        /// </remarks>
        public Client(TcpClient client)
        {
            _client = client;

            // get the ip address of the client to register him with our client list
            _clientIPEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            _clientAddress = _clientIPEndPoint.Address;
            _clientIP = client.Client.RemoteEndPoint.ToString();

            Logger.LogUserLogIn($"A user has established a connection to the server with the following IP: {_clientIP}.");

            // Add the new client to our clients collection
            AllClients.Add(_clientIP, this);

            // Read data from the client async
            data = new byte[_client.ReceiveBufferSize];
            dataHistory = new byte[0];

            // BeginRead will begin async read from the NetworkStream.
            // This allows the server to remain responsive and continue accepting new connections from other clients.
            // When reading complete control will be transfered to the ReviveMessage() function.
            _client.GetStream().BeginRead(data, 0, 4, ReceiveMessageLength, null);

            _isClientConnected = true;
            smtpHandler = new SmtpHandler();
            encryptionExpirationDate = new EncryptionExpirationDate(this);
            captchaCodeHandler = new CaptchaCodeHandler();
            captchaRotatingImageHandler = new CaptchaRotatingImageHandler();
            _LoginFailedAttempts = new ClientAttemptsState(this,EnumHandler.UserAuthentication_Enum.Login);
            _isOnline = false;
            _inCall = false;
        }

        #endregion

        #region Private Failed_Attempts Methods

        /// <summary>
        /// The "HandleFailedAttempt" method handles a failed login attempt.
        /// </summary>
        /// <param name="clientAttemptsState">The ClientAttemptsState object representing the client's login attempts state.</param>
        /// <param name="BanStart">The CommunicationMessageID_Enum indicating the start of a ban.</param>
        /// <param name="FailedAttempt">The action to take when the login attempt fails.</param>
        /// <remarks>
        /// This method increments the failed login attempt count for the client.
        /// If the client reaches the maximum number of failed attempts and becomes banned, it logs the ban and sends a ban message to the client.
        /// If the client is not banned, it performs the action specified in "FailedAttempt".
        /// </remarks>
        private void HandleFailedAttempt(ClientAttemptsState clientAttemptsState, EnumHandler.CommunicationMessageID_Enum BanStart, Action FailedAttempt)
        {
            clientAttemptsState.HandleFailedAttempt();
            if (clientAttemptsState.IsUserBanned())
            {
                clientAttemptsState.HandleBan();
                Logger.LogUserBanStart($"A user has been blocked from the server with the following IP: {_clientIP}.");

                double banDuration = clientAttemptsState.CurrentBanDuration;
                EnumHandler.CommunicationMessageID_Enum messageType = BanStart;
                object messageContent = banDuration;
                SendMessage(messageType, messageContent);
            }
            else
            {
                FailedAttempt();
            }
        }

        /// <summary>
        /// The "HandleFailedAttempt" method handles a failed login attempt.
        /// </summary>
        /// <param name="clientAttemptsState">The ClientAttemptsState object representing the client's login attempts state.</param>
        /// <param name="BanStart">The CommunicationMessageID_Enum indicating the start of a ban.</param>
        /// <param name="username">The username of the client whose login attempt failed.</param>
        /// <param name="FailedAttempt">The action to take when the login attempt fails, including the username.</param>
        /// <remarks>
        /// This method increments the failed login attempt count for the client.
        /// If the client reaches the maximum number of failed attempts and becomes banned, it logs the ban and sends a ban message to the client.
        /// If the client is not banned, it performs the action specified in "FailedAttempt" with the username.
        /// </remarks>
        private void HandleFailedAttempt(ClientAttemptsState clientAttemptsState, EnumHandler.CommunicationMessageID_Enum BanStart, string username, Action<string> FailedAttempt)
        {
            clientAttemptsState.HandleFailedAttempt();
            if (clientAttemptsState.IsUserBanned())
            {
                clientAttemptsState.HandleBan();
                Logger.LogUserBanStart($"A user has been blocked from the server with the following IP: {_clientIP}.");
                double banDuration = clientAttemptsState.CurrentBanDuration;
                EnumHandler.CommunicationMessageID_Enum messageType = BanStart;
                object messageContent = banDuration;
                SendMessage(messageType, messageContent);
            }
            else
            {
                FailedAttempt(username);
            }
        }

        /// <summary>
        /// The "HandleFailedAttempt" method handles a failed login attempt.
        /// </summary>
        /// <param name="clientAttemptsState">The ClientAttemptsState object representing the client's login attempts state.</param>
        /// <param name="BanStart">The CommunicationMessageID_Enum indicating the start of a ban.</param>
        /// <param name="username">The username of the client whose login attempt failed.</param>
        /// <param name="emailAddress">The email address of the client whose login attempt failed.</param>
        /// <param name="FailedAttempt">The action to take when the login attempt fails, including the username and email address.</param>
        /// <remarks>
        /// This method increments the failed login attempt count for the client.
        /// If the client reaches the maximum number of failed attempts and becomes banned, it logs the ban and sends a ban message to the client.
        /// If the client is not banned, it performs the action specified in "FailedAttempt" with the username and email address.
        /// </remarks>
        private void HandleFailedAttempt(ClientAttemptsState clientAttemptsState, EnumHandler.CommunicationMessageID_Enum BanStart, string username, string emailAddress, Action<string, string> FailedAttempt)
        {
            clientAttemptsState.HandleFailedAttempt();
            if (clientAttemptsState.IsUserBanned())
            {
                clientAttemptsState.HandleBan();
                Logger.LogUserBanStart($"A user has been blocked from the server with the following IP: {_clientIP}.");
                double banDuration = clientAttemptsState.CurrentBanDuration;
                EnumHandler.CommunicationMessageID_Enum messageType = BanStart;
                object messageContent = banDuration;
                SendMessage(messageType, messageContent);
            }
            else
            {
                FailedAttempt(username, emailAddress);
            }
        }

        /// <summary>
        /// The "HandleFailedAttempt" method handles a failed login attempt.
        /// </summary>
        /// <param name="clientAttemptsState">The ClientAttemptsState object representing the client's login attempts state.</param>
        /// <param name="BanStartEnum">The CommunicationMessageID_Enum indicating the start of a ban.</param>
        /// <param name="FailedAttemptEnum">The CommunicationMessageID_Enum indicating the type of failed attempt.</param>
        /// <param name="FailedAttempt">The action to take when the login attempt fails, including the type of failed attempt.</param>
        /// <remarks>
        /// This method increments the failed login attempt count for the client.
        /// If the client reaches the maximum number of failed attempts and becomes banned, it logs the ban and sends a ban message to the client.
        /// If the client is not banned, it performs the action specified in "FailedAttempt" with the type of failed attempt.
        /// </remarks>
        private void HandleFailedAttempt(ClientAttemptsState clientAttemptsState, EnumHandler.CommunicationMessageID_Enum BanStartEnum, EnumHandler.CommunicationMessageID_Enum FailedAttemptEnum, Action<EnumHandler.CommunicationMessageID_Enum> FailedAttempt)
        {
            clientAttemptsState.HandleFailedAttempt();
            if (clientAttemptsState.IsUserBanned())
            {
                clientAttemptsState.HandleBan();
                Logger.LogUserBanStart($"A user has been blocked from the server with the following IP: {_clientIP}.");
                double banDuration = clientAttemptsState.CurrentBanDuration;
                EnumHandler.CommunicationMessageID_Enum messageType = BanStartEnum;
                object messageContent = banDuration;
                SendMessage(messageType, messageContent);
            }
            else
            {
                FailedAttempt(FailedAttemptEnum);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The "HandleEncryptionClientPublicKeySenderEnum" method handles the reception of the client's public key during encryption setup.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the client's public key.</param>
        /// <remarks>
        /// This method sets the ClientPublicKey property to the received public key.
        /// It generates a new RSAServiceProvider instance and obtains the server's public key.
        /// The server's public key is then sent to the client for encryption purposes.
        /// A symmetric key is generated and encrypted with the client's public key, then sent to the client.
        /// Finally, the encryption expiration timer is started.
        /// </remarks>
        private void HandleEncryptionClientPublicKeySenderEnum(JsonObject jsonObject)
        {
            // Set client's public key
            ClientPublicKey = jsonObject.MessageBody as string;

            // Create RSA service provider and get server's public key
            Rsa = new RSAServiceProvider();
            string serverPublicKey = Rsa.GetPublicKey();

            // Send server's public key to client
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.EncryptionServerPublicKeyReciever;
            object messageContent = serverPublicKey;
            SendMessage(messageType, messageContent, false);

            // Generate and encrypt symmetric key
            SymmetricKey = RandomStringCreator.RandomString(32);
            string EncryptedSymmetricKey = Rsa.Encrypt(SymmetricKey, ClientPublicKey);
            messageType = EnumHandler.CommunicationMessageID_Enum.EncryptionSymmetricKeyReciever;
            messageContent = EncryptedSymmetricKey;
            SendMessage(messageType, messageContent, false);

            // Start encryption expiration timer
            encryptionExpirationDate.Start();
        }

        /// <summary>
        /// The "HandleLoginRequestEnum" method handles the login request from the client.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the login details.</param>
        /// <remarks>
        /// This method extracts the username and password from the login details.
        /// It checks if the username and password match an existing user and if the user is not already connected.
        /// If the login is successful, it sets the client's nickname to the username and sends a login code to the user's email address.
        /// If the login fails, it handles the failed attempt, possibly leading to a ban if too many failed attempts occur.
        /// </remarks>
        private void HandleLoginRequestEnum(JsonObject jsonObject)
        {
            LoginDetails loginDetails = jsonObject.MessageBody as LoginDetails;
            string username = loginDetails.Username;
            string password = loginDetails.Password;
            if ((DataHandler.IsMatchingUsernameAndPasswordExist(username, password)) && (!UserIsConnected(username)))
            {
                _ClientNick = username;
                string emailAddress = DataHandler.GetEmailAddress(_ClientNick);
                if (emailAddress != "")
                {
                    smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.LoginMessage);
                    _LoginFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Login);

                    EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.loginResponse_SmtpLoginMessage;
                    object messageContent = null;
                    SendMessage(messageType, messageContent);
                }
                else // shouldn't get here - email address won't be empty...
                {
                    SendFailedLoginMessage();
                }
            }
            else
            {
                HandleFailedAttempt(_LoginFailedAttempts, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SendFailedLoginMessage);
            }
        }

        /// <summary>
        /// The "HandleloginRequest_SmtpLoginMessage" method handles a login request with an SMTP login message.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the login request information.</param>
        /// <remarks>
        /// This method extracts the username and a flag indicating whether the request follows a failed attempt.
        /// If the request follows a failed attempt, it handles the SMTP login message.
        /// Otherwise, it processes the login attempt as a failed attempt, potentially leading to a ban.
        /// </remarks>
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

        /// <summary>
        /// The "HandleSmtpLoginMessage" method handles the SMTP login message for a specified username.
        /// </summary>
        /// <param name="username">The username for which the SMTP login message is handled.</param>
        /// <remarks>
        /// This method retrieves the email address associated with the username from the data handler.
        /// It then sends a login message to the user's email address using the SMTP handler.
        /// Finally, it sends a response message to the client indicating that the SMTP login message has been processed.
        /// </remarks>
        private void HandleSmtpLoginMessage(string username)
        {
            string emailAddress = DataHandler.GetEmailAddress(_ClientNick);
            if (emailAddress != "")
            {
                smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.LoginMessage);
            }
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.loginResponse_SmtpLoginMessage;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleRegistrationRequest_SmtpRegistrationCodeEnum" method handles the registration request with an SMTP registration code.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the registration code.</param>
        /// <remarks>
        /// This method retrieves the registration code from the message body of the JSON object.
        /// It compares the registration code with the SMTP code stored in the SMTP handler.
        /// If the codes match, it sends a successful registration response to the client.
        /// If the codes do not match, it increments the failed SMTP registration attempts count.
        /// If the maximum number of failed attempts is reached and the client becomes banned, it sends a ban message to the client.
        /// </remarks>
        private void HandleRegistrationRequest_SmtpRegistrationCodeEnum(JsonObject jsonObject)
        {
            string code = jsonObject.MessageBody as string;
            if (smtpHandler.GetSmtpCode() == code)
            {
                _RegistrationSmtpFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Registration);

                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_SuccessfulSmtpRegistrationCode;
                object messageContent = null;
                SendMessage(messageType, messageContent);
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

        /// <summary>
        /// The "SendFailedSmtpRegistrationCode" method sends a failed SMTP registration code response to the client.
        /// </summary>
        private void SendFailedSmtpRegistrationCode()
        {
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_FailedSmtpRegistrationCode;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "SendFailedSmtpPasswordRestartCode" method sends a failed SMTP password restart code response to the client.
        /// </summary>
        private void SendFailedSmtpPasswordRestartCode()
        {
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.FailedResetPasswordResponse_SmtpCode;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleResetPasswordRequest_SmtpCodeEnum" method handles a reset password request with an SMTP code.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the reset password request.</param>
        /// <remarks>
        /// This method checks if the provided SMTP code matches the expected code.
        /// If the code matches, it initializes the failed password reset attempts state and sends a successful reset password response.
        /// If the code does not match, it initializes the failed password reset attempts state and sends a failed SMTP password restart code response.
        /// </remarks>
        private void HandleResetPasswordRequest_SmtpCodeEnum(JsonObject jsonObject)
        {
            string code = jsonObject.MessageBody as string;
            if (smtpHandler.GetSmtpCode() == code)
            {
                _PasswordResetFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.PasswordRestart);
                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.SuccessfulResetPasswordResponse_SmtpCode;
                object messageContent = null;
                SendMessage(messageType, messageContent);
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

        /// <summary>
        /// The "HandleResetPasswordRequestEnum" method handles a reset password request.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the reset password request details.</param>
        /// <remarks>
        /// This method verifies if the provided username and email address match an existing user.
        /// If the username and email address match, it initializes the failed password reset attempts state,
        /// sends a password renewal message to the user's email, and responds with a successful reset password response.
        /// If the username and email address do not match, it initializes the failed password reset attempts state
        /// and sends a failed reset password response.
        /// </remarks>
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

                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.SuccessfulResetPasswordResponse;
                object messageContent = null;
                SendMessage(messageType, messageContent);
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

        /// <summary>
        /// The "SendFailedResetPasswordResponse" method sends a failed reset password response.
        /// </summary>
        private void SendFailedResetPasswordResponse()
        {
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.FailedResetPasswordResponse;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleFriendRequestSenderEnum" method processes a friend request sent by the client.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the friend request details.</param>
        /// <remarks>
        /// This method checks if the friend request sender is not the same as the receiver.
        /// It then verifies if the receiver's username and tagline exist in the system.
        /// If the sender and receiver are not already friends and there is no pending request from either side,
        /// a friend request is added to the database and an alert email is sent to the receiver.
        /// If the receiver is online, a notification is sent immediately; otherwise, it is sent when the receiver logs in.
        /// </remarks>
        private void HandleFriendRequestSenderEnum(JsonObject jsonObject)
        {
            FriendRequestDetails friendRequestDetails = jsonObject.MessageBody as FriendRequestDetails;
            string FriendRequestReceiverUsername = friendRequestDetails.Username;
            string FriendRequestReceiverTagLine = friendRequestDetails.TagLine;
            string FriendRequestSenderUsername = _ClientNick;

            if (FriendRequestSenderUsername != FriendRequestReceiverUsername)
            {
                if (DataHandler.IsMatchingUsernameAndTagLineIdExist(FriendRequestReceiverUsername, FriendRequestReceiverTagLine))
                {
                    if (!DataHandler.AreFriends(FriendRequestSenderUsername, FriendRequestReceiverUsername))
                    {
                        if (DataHandler.IsFriendRequestPending(FriendRequestSenderUsername, FriendRequestReceiverUsername)) //user already send one
                        {
                            // there is nothing to do
                        }
                        else if (DataHandler.IsFriendRequestPending(FriendRequestReceiverUsername, FriendRequestSenderUsername)) //the other user already send to you
                        {
                            HandleFriendRequestResponse(FriendRequestReceiverUsername, FriendRequestSenderUsername, ApprovalFriendRequestResponse);
                        }
                        else
                        {
                            if (DataHandler.AddFriendRequest(FriendRequestSenderUsername, FriendRequestReceiverUsername) > 0)
                            {
                                string emailAddress = DataHandler.GetEmailAddress(FriendRequestReceiverUsername);
                                if (emailAddress != "")
                                {
                                    smtpHandler.SendFriendRequestAlertToUserEmail(FriendRequestReceiverUsername, FriendRequestSenderUsername, emailAddress);
                                }

                                string profilePicture = DataHandler.GetProfilePicture(FriendRequestSenderUsername);
                                DateTime currentTime = DateTime.Now;
                                DateTime requestDate = DataHandler.GetFriendRequestDate(FriendRequestSenderUsername, FriendRequestReceiverUsername, currentTime);

                                if (profilePicture != "" && requestDate != currentTime)
                                {
                                    PastFriendRequest friendRequest = new PastFriendRequest(FriendRequestSenderUsername, profilePicture, requestDate);

                                    EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.FriendRequestReciever;
                                    object messageContent = friendRequest;
                                    Unicast(messageType, messageContent, FriendRequestReceiverUsername);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The "HandleFriendRequestResponseSenderEnum" method processes a friend request response sent by the client.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the friend request response details.</param>
        /// <remarks>
        /// This method extracts the details of the friend request response, including the sender's username and the response status.
        /// It then calls the "HandleFriendRequestResponse" method to handle the friend request response.
        /// </remarks>
        private void HandleFriendRequestResponseSenderEnum(JsonObject jsonObject) //todo - not finished...
        {
            FriendRequestResponseDetails friendRequestResponseDetails = jsonObject.MessageBody as FriendRequestResponseDetails;
            string FriendRequestSenderUsername = friendRequestResponseDetails.Username;
            string FriendRequestReceiverUsername = _ClientNick;
            string FriendRequestStatus = friendRequestResponseDetails.Status;
            HandleFriendRequestResponse(FriendRequestSenderUsername, FriendRequestReceiverUsername, FriendRequestStatus);
        }

        /// <summary>
        /// The "HandleFriendRequestResponse" method processes a friend request response between two users.
        /// </summary>
        /// <param name="FriendRequestSenderUsername">The username of the user who sent the friend request.</param>
        /// <param name="FriendRequestReceiverUsername">The username of the user who received the friend request.</param>
        /// <param name="FriendRequestStatus">The status of the friend request (e.g., approved or rejected).</param>
        /// <remarks>
        /// This method handles the friend request response by updating the database with the status of the request.
        /// If the request is approved, it creates a direct chat between the two users and adds them as friends.
        /// If the request is rejected, no action is taken.
        /// </remarks>
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
                    XmlFileManager xmlFileManager = new XmlFileManager(xmlFileName, chatParticipantNames, ChatTagLine);
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

                        EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.FriendRequestResponseReciever;
                        object messageContent = friendRequestSenderUsernameContactAndChat;
                        SendMessage(messageType, messageContent);

                        ContactDetails friendRequestReceiverUsernameContact = DataHandler.GetFriendProfileInformation(FriendRequestReceiverUsername);
                        ContactAndChat friendRequestReceiverUsernameContactAndChat = new ContactAndChat(directChat, friendRequestReceiverUsernameContact);

                        messageContent = friendRequestReceiverUsernameContactAndChat;
                        Unicast(messageType, messageContent, FriendRequestSenderUsername);
                    }
                    else
                    {
                        xmlFileManager.DeleteFile();
                    }
                }
            }
        }

        /// <summary>
        /// The "HandleRegistrationRequest_SmtpRegistrationMessageEnum" method processes a registration request with an SMTP registration message.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the registration details.</param>
        /// <remarks>
        /// This method extracts the username and email address from the registration request.
        /// If the request is a retry after a failed attempt, it sends the SMTP registration message again.
        /// If the request is not a retry, it handles the failed attempt and sends the SMTP registration message.
        /// </remarks>
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

        /// <summary>
        /// The "SmtpRegistrationMessage" method sends a registration message to the user's email address.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="emailAddress">The email address of the user.</param>
        /// <remarks>
        /// This method generates and sends an SMTP registration message to the specified email address.
        /// It then sends a registration response message to the client.
        /// </remarks>
        private void SmtpRegistrationMessage(string username, string emailAddress)
        {
            smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.RegistrationMessage);

            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_SmtpRegistrationMessage;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleSmtpResetPasswordMessage" method sends a password renewal message to the user's email address.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="emailAddress">The email address of the user.</param>
        /// <remarks>
        /// This method generates and sends an SMTP password renewal message to the specified email address.
        /// It then sends a reset password response message to the client.
        /// </remarks>
        private void HandleSmtpResetPasswordMessage(string username, string emailAddress)
        {
            smtpHandler.SendCodeToUserEmail(username, emailAddress, EnumHandler.SmtpMessageType_Enum.PasswordRenewalMessage);

            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.ResetPasswordResponse_SmtpMessage;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleResetPasswordRequest_SmtpMessageEnum" method handles a request to reset a user's password using an SMTP message.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the SMTP details for password reset.</param>
        /// <remarks>
        /// This method processes the SMTP details from the JSON object to determine if a password reset is requested.
        /// If the request is successful, it sends a password renewal message to the user's email address.
        /// If the request fails, it handles the failed attempt and possibly initiates a ban on the user.
        /// </remarks>
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
                HandleFailedAttempt(_PasswordResetFailedAttempts, EnumHandler.CommunicationMessageID_Enum.ResetPasswordBanStart, username, emailAddress, HandleSmtpResetPasswordMessage);
            }
        }

        /// <summary>
        /// The "HandleRegistrationRequest_RegistrationEnum" method handles a registration request.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the registration information.</param>
        /// <remarks>
        /// This method processes the registration information from the JSON object, including username, password, first name, last name,
        /// email address, city name, date of birth, registration date, gender, and verification questions and answers.
        /// It checks if the username or the email address already exists in the database and handles the registration process accordingly.
        /// If the registration is successful, it sets the client's username and sends a successful registration response message.
        /// If the registration fails, it handles the failed attempt and possibly initiates a ban on the user.
        /// </remarks>
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

            if (!DataHandler.UsernameIsExist(username) && !DataHandler.EmailAddressIsExist(emailAddress))
            {
                EnumHandler.CommunicationMessageID_Enum RegistrationResponseEnum;
                if (DataHandler.InsertUser(username, password, firstName, lastName, emailAddress, cityName, gender, dateOfBirthAsString, registrationDateAsString, VerificationQuestionsAndAnswers) > 0)
                {
                    _RegistrationFailedAttempts = null;
                    _RegistrationSmtpFailedAttempts = null;

                    _ClientNick = username;
                    RegistrationResponseEnum = EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_SuccessfulRegistration;
                }
                else
                {
                    RegistrationResponseEnum = EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_FailedRegistration;
                }
                EnumHandler.CommunicationMessageID_Enum messageType = RegistrationResponseEnum;
                object messageContent = null;
                SendMessage(messageType, messageContent);
            }
            else
            {
                if (_RegistrationFailedAttempts == null)
                {
                    _RegistrationFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Registration);
                }
                HandleFailedAttempt(_RegistrationFailedAttempts, EnumHandler.CommunicationMessageID_Enum.RegistrationBanStart, SendFailedRegistration);
            }
        }

        /// <summary>
        /// The "SendFailedRegistration" method sends a message indicating that the registration process has failed.
        /// </summary>
        /// <remarks>
        /// This method is called when a registration attempt fails due to an existing username or email address in the database.
        /// It sends a message to the client with a failed registration response.
        /// </remarks>
        private void SendFailedRegistration()
        {
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.RegistrationResponse_FailedRegistration;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleRegistrationRequest_UploadProfilePictureRequest" method handles the request to upload a profile picture during the registration process.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the profile picture ID.</param>
        /// <remarks>
        /// This method inserts the profile picture ID into the database for the current client.
        /// If successful, it sends a response message with the uploaded profile picture ID.
        /// </remarks>
        private void HandleRegistrationRequest_UploadProfilePictureRequest(JsonObject jsonObject) //todo - handle the else statment...
        {
            string profilePictureId = jsonObject.MessageBody as string;
            if (DataHandler.InsertProfilePicture(_ClientNick, profilePictureId) > 0)
            {
                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.UploadProfilePictureResponse;
                object messageContent = profilePictureId;
                SendMessage(messageType, messageContent);
            }
        }

        /// <summary>
        /// The "HandleRegistrationRequest_UploadStatusRequest" method handles the request to upload a status message during the registration process.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the status message.</param>
        /// <remarks>
        /// This method inserts the status message into the database for the current client.
        /// If successful, it sends a response message with the uploaded status message.
        /// </remarks>
        private void HandleRegistrationRequest_UploadStatusRequest(JsonObject jsonObject)
        {
            string status = jsonObject.MessageBody as string;
            if (DataHandler.InsertStatus(_ClientNick, status) > 0)
            {
                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.UploadStatusResponse;
                object messageContent = status;
                SendMessage(messageType, messageContent);
            }
        }

        /// <summary>
        /// The "SendFailedLoginMessage" method sends a failed login response message to the client.
        /// </summary>
        private void SendFailedLoginMessage()
        {
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.loginResponse_FailedLogin;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleLoginRequest_SmtpLoginCode" method handles a login request with an SMTP login code.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the login request information.</param>
        /// <remarks>
        /// This method checks if the SMTP code provided in the login request matches the expected SMTP code.
        /// If the code is correct, it creates a captcha image and sends it to the client for authentication.
        /// If the code is incorrect, it handles the failed login attempt.
        /// </remarks>
        private void HandleLoginRequest_SmtpLoginCode(JsonObject jsonObject)
        {
            string code = jsonObject.MessageBody as string;

            if (smtpHandler.GetSmtpCode() == code)
            {
                Image catpchaBitmap = captchaCodeHandler.CreateCatpchaBitmap();
                byte[] bytes = ConvertHandler.ConvertImageToBytes(catpchaBitmap);
                ImageContent imageContent = new ImageContent(bytes);
                _LoginFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Login);
                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.LoginResponse_SuccessfulSmtpLoginCode;
                object messageContent = imageContent;
                SendMessage(messageType, messageContent);
            }    
            else
            {
                HandleFailedAttempt(_LoginFailedAttempts, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SendFailedLoginSmtpCode);
            }
        }

        /// <summary>
        /// The "SendFailedLoginSmtpCode" method sends a failed SMTP login code response to the client.
        /// </summary>
        /// <remarks>
        /// This method is called when the SMTP login code provided by the client is incorrect.
        /// It sends a message to the client indicating the failure of the login attempt.
        /// </remarks>
        private void SendFailedLoginSmtpCode()
        {
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.LoginResponse_FailedSmtpLoginCode;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleCaptchaImageRequestEnum" method handles a request for a CAPTCHA image.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the request.</param>
        /// <remarks>
        /// This method checks if the request is made after a failed attempt to login.
        /// If it is, it generates and sends a CAPTCHA image to the client for verification.
        /// If not, it handles the failed attempt by incrementing the failed login attempt count and possibly banning the client.
        /// </remarks>
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

        /// <summary>
        /// The "HandleCaptchaImage" method generates and sends a CAPTCHA image to the client for verification.
        /// </summary>
        /// <remarks>
        /// This method creates a CAPTCHA image using a CAPTCHA code handler and converts it to bytes.
        /// The byte array is then encapsulated in an ImageContent object and sent to the client as a response.
        /// </remarks>
        private void HandleCaptchaImage()
        {
            Image catpchaBitmap = captchaCodeHandler.CreateCatpchaBitmap();
            byte[] bytes = ConvertHandler.ConvertImageToBytes(catpchaBitmap);
            ImageContent captchaBitmapContent = new ImageContent(bytes);

            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.CaptchaImageResponse;
            object messageContent = captchaBitmapContent;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleDisconnectEnum" method handles the disconnection of a client from the server.
        /// </summary>
        /// <remarks>
        /// This method sets the user's status to offline in the database and notifies the user's friends about the status change.
        /// It then closes the network stream, disposes of the client resources, and removes the client from the list of connected clients.
        /// </remarks>
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
                    

                    EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.OfflineUpdate;
                    object messageContent = offlineDetails;
                    foreach (string friendName in friendNames)
                    {
                        Unicast(messageType, messageContent, friendName);
                    }
                }
            }
            NetworkStream stream = _client.GetStream();
            stream.Close();
            _client.Close();
            _client.Dispose();
            _client = null;
            _isClientConnected = false;
            AllClients.Remove(_clientIP);
            Logger.LogUserLogOut($"A user has logged out from the server with the following IP: {_clientIP}.");
        }

        /// <summary>
        /// The "HandleCaptchaCodeRequestEnum" method handles the verification of a captcha code provided by the client.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the captcha code to be verified.</param>
        /// <remarks>
        /// This method compares the provided captcha code with the expected code. If the codes match, it sends the client details of the captcha rotation image and resets the login failed attempts.
        /// If the codes do not match, it increments the login failed attempts and may trigger a ban depending on the number of failed attempts.
        /// </remarks>
        private void HandleCaptchaCodeRequestEnum(JsonObject jsonObject)
        {
            string code = jsonObject.MessageBody as string;
            if (captchaCodeHandler.CompareCode(code))
            {
                CaptchaRotationImageDetails captchaRotationImageDetails = GetCaptchaRotationImageDetails();
                _LoginFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Login);

                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.SuccessfulCaptchaCodeResponse;
                object messageContent = captchaRotationImageDetails;
                SendMessage(messageType, messageContent);
            }
            else
            {
                HandleFailedAttempt(_LoginFailedAttempts, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SendFailedCaptchaCode);
            }
        }

        /// <summary>
        /// The "SendFailedCaptchaCode" method sends a response to the client indicating that the captcha code verification has failed.
        /// </summary>
        private void SendFailedCaptchaCode()
        {
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.FailedCaptchaCodeResponse;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleDeleteMessageRequestEnum" method handles a request to delete a message from a chat.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the message to be deleted.</param>
        /// <remarks>
        /// This method retrieves the message to be deleted from the JsonObject and verifies that the sender
        /// of the message is the same as the current client. If so, it retrieves the necessary information
        /// such as the chat ID, message date and time, and message content. It then updates the chat's XML
        /// file to mark the message as deleted and updates the database to reflect the deleted message.
        /// Finally, it sends a delete message response to the chat.
        /// </remarks>
        private void HandleDeleteMessageRequestEnum(JsonObject jsonObject)
        {
            JsonClasses.Message message = jsonObject.MessageBody as JsonClasses.Message;
            string messageSenderName = message.MessageSenderName;
            if (messageSenderName == _ClientNick)
            {
                string chatId = message.ChatId;

                DateTime messageDateTime = message.MessageDateAndTime;
                XmlFileManager xmlFileManager = ChatHandler.ChatHandler.ChatFileManagers[chatId];
                object messageContentValue = message.MessageContent;

                ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
                string lastMessageContentValue = "";
                string messageTypeValue = "";
                string messageContentAsString = "";

                if (messageContentValue is string textMessageContent)
                {
                    messageTypeValue = "Text";
                    messageContentAsString = textMessageContent;
                    lastMessageContentValue = textMessageContent;
                }
                else if (messageContentValue is ImageContent imageMessageContent)
                {
                    messageTypeValue = "Image";
                    byte[] imageMessageContentByteArray = imageMessageContent.ImageBytes;
                    string imageMessageContentString = Convert.ToBase64String(imageMessageContentByteArray);
                    messageContentAsString = imageMessageContentString;
                    lastMessageContentValue = "Image";
                }

                if (messageDateTime.CompareTo(chat.LastMessageTime) == 0 && messageSenderName == chat.LastMessageSenderName)
                {
                    chat.LastMessageContent = "Deleted Message";
                }
                xmlFileManager.EditMessage(messageSenderName, messageTypeValue, messageContentAsString, messageDateTime);

                string TableName = "";
                if (chat is DirectChatDetails)
                    TableName = "DirectChats";
                else if (chat is GroupChatDetails)
                    TableName = "GroupChats";
                string messageDateTimeAsString = messageDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                if (DataHandler.AreLastMessageDataIdentical(TableName, chatId, lastMessageContentValue, messageSenderName, messageDateTimeAsString))
                {
                    DataHandler.UpdateLastMessageData(TableName, chatId, "Deleted Message", messageSenderName, messageDateTimeAsString);
                }

                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.DeleteMessageResponse;
                object messageContent = message;
                SendChatMessage(messageType, messageContent, chatId);
            }        
        }

        /// <summary>
        /// The "HandleSendMessageRequestEnum" method handles a request to send a message in a chat.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the message to be sent.</param>
        /// <remarks>
        /// This method retrieves the message to be sent from the JsonObject and verifies that the sender
        /// of the message is the same as the current client. If so, it retrieves the necessary information
        /// such as the chat ID, message date and time, and message content. It updates the chat's last
        /// message details and XML file to include the new message. Finally, it sends a response to confirm
        /// that the message has been sent.
        /// </remarks>
        private void HandleSendMessageRequestEnum(JsonObject jsonObject)
        {
            JsonClasses.Message message = jsonObject.MessageBody as JsonClasses.Message;
            string messageSenderName = message.MessageSenderName;
            if (messageSenderName == _ClientNick)
            {
                string chatId = message.ChatId;

                DateTime messageDateTime = message.MessageDateAndTime;
                XmlFileManager xmlFileManager = ChatHandler.ChatHandler.ChatFileManagers[chatId];
                object messageContentValue = message.MessageContent;

                ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
                chat.LastMessageTime = messageDateTime;
                string lastMessageContentValue = "";
                string messageTypeValue = "";
                string messageContentAsString = "";

                if (messageContentValue is string textMessageContent)
                {
                    messageTypeValue = "Text";
                    messageContentAsString = textMessageContent;
                    lastMessageContentValue = textMessageContent;
                }
                else if (messageContentValue is ImageContent imageMessageContent)
                {
                    messageTypeValue = "Image";
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
                    xmlFileManager.AppendMessage(messageSenderName, messageTypeValue, messageContentAsString, messageDateTime);

                    EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.SendMessageResponse;
                    object messageContent = message;
                    SendChatMessage(messageType, messageContent, chatId);
                }           
            }      
        }

        /// <summary>
        /// The "HandlePasswordUpdateRequestEnum" method handles a request to update a user's password.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the password update details.</param>
        /// <remarks>
        /// This method retrieves the username, past password, and new password from the JsonObject.
        /// It checks if the provided username and past password match an existing user's credentials.
        /// If they do, it creates a PasswordRenewalOptions object with success, failure, and error
        /// response messages. It then calls the HandlePasswordRenewal method to update the password.
        /// </remarks>
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
            if (!DataHandler.IsMatchingUsernameAndPasswordExist(username, pastPassword)) 
            {
                HandleFailedAttempt(_PasswordUpdateFailedAttempts, EnumHandler.CommunicationMessageID_Enum.PasswordUpdateBanStart, SendUnmatchedDetailsPasswordUpdateResponse);
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

        /// <summary>
        /// The "SendUnmatchedDetailsPasswordUpdateResponse" method sends a response indicating that the provided username and past password do not match.
        /// </summary>
        private void SendUnmatchedDetailsPasswordUpdateResponse()
        {
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.FailedPasswordUpdateResponse_UnmatchedDetails;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandlePasswordRenewalMessageRequestEnum" method handles a request to renew a user's password.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the password renewal details.</param>
        /// <remarks>
        /// This method retrieves the username and new password from the JsonObject.
        /// It creates a PasswordRenewalOptions object with success, failure, and error
        /// response messages. It then calls the HandlePasswordRenewal method to renew the password.
        /// </remarks>
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

        /// <summary>
        /// The "SendFailedPasswordRenewal" method sends a failed password renewal response message.
        /// </summary>
        /// <param name="failedPasswordRenewalEnum">The CommunicationMessageID_Enum value for the failed renewal response.</param>
        private void SendFailedPasswordRenewal(EnumHandler.CommunicationMessageID_Enum failedPasswordRenewalEnum)
        {
            EnumHandler.CommunicationMessageID_Enum messageType = failedPasswordRenewalEnum;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandlePasswordRenewal" method handles the renewal of a user's password.
        /// </summary>
        /// <param name="username">The username of the user whose password is being renewed.</param>
        /// <param name="password">The new password for the user.</param>
        /// <param name="passwordRenewalOptions">The options for handling successful, failed, and error password renewal responses.</param>
        /// <param name="clientAttemptsState">The state of the client's password renewal attempts.</param>
        /// <remarks>
        /// This method checks if the provided username and password combination already exist,
        /// indicating that the password has been chosen before by the user. If the password already
        /// exists, it retrieves the appropriate failed password renewal message type based on the
        /// client's authentication state and sends a failed attempt message. If the password does not
        /// already exist, it sets the new password for the user and sends a successful password renewal
        /// message. The method also handles the case where the user's password renewal fails due to an
        /// error, sending an error password renewal message.
        /// </remarks>
        private void HandlePasswordRenewal(string username, string password, PasswordRenewalOptions passwordRenewalOptions, ClientAttemptsState clientAttemptsState)
        {
            if (DataHandler.PasswordIsExist(username, password))
            {
                EnumHandler.CommunicationMessageID_Enum failedPasswordRenewalEnum = passwordRenewalOptions.GetFailedPasswordRenewal();
                EnumHandler.CommunicationMessageID_Enum BanStartEnumType;
                switch (clientAttemptsState.UserAuthenticationState)
                {
                    case EnumHandler.UserAuthentication_Enum.PasswordRestart:
                        BanStartEnumType = EnumHandler.CommunicationMessageID_Enum.ResetPasswordBanStart;
                        break;
                    default:
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
                EnumHandler.CommunicationMessageID_Enum messageType = passwordRenewalEnumType;
                object messageContent = null;
                SendMessage(messageType, messageContent);
            }
        }

        /// <summary>
        /// The "HandleUdpAudioConnectionRequestEnum" method handles a request to establish a UDP audio connection.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the UDP audio connection request details.</param>
        /// <remarks>
        /// This method parses the port number from the JsonObject message body and updates the UDP endpoint for audio communication.
        /// It generates a random symmetric key for encrypting audio data and adds it to the clientKeys collection.
        /// The method then sends a response message containing the encrypted symmetric key for the audio connection.
        /// </remarks>
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

            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.UdpAudioConnectionResponse;
            object messageContent = EncryptedSymmerticKey;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleUdpVideoConnectionRequestEnum" method handles a request to establish a UDP video connection.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the UDP video connection request details.</param>
        /// <remarks>
        /// This method extracts the audio and video ports from the JsonObject message body and creates
        /// new UDP endpoints for audio and video communication.
        /// It generates random symmetric keys for encrypting audio and video data and adds them to the clientKeys collection.
        /// The method then sends a response message containing the encrypted symmetric keys for the audio and video connections.
        /// </remarks>
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
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.UdpVideoConnectionResponse;
            object messageContent = udpDetails;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleUpdateProfileStatusRequestEnum" method handles a request to update the user's profile status.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the new status.</param>
        /// <remarks>
        /// This method inserts the new status into the database for the current user.
        /// If the insertion is successful, it creates a StatusUpdate object with the user's nickname and the new status.
        /// It then retrieves the list of friends for the current user and sends an update message to each friend with the new status.
        /// Finally, it sends a response message to the sender confirming the status update.
        /// </remarks>
        private void HandleUpdateProfileStatusRequestEnum(JsonObject jsonObject)
        {
            string status = jsonObject.MessageBody as string;
            if (DataHandler.InsertStatus(_ClientNick, status) > 0)
            {
                StatusUpdate statusUpdate = new StatusUpdate(_ClientNick, status);
                List<string> friendNames = DataHandler.GetFriendList(_ClientNick);

                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.UpdateProfileStatusResponse_Sender;
                object messageContent = status;
                SendMessage(messageType, messageContent);

                messageType = EnumHandler.CommunicationMessageID_Enum.UpdateProfileStatusResponse_Reciever;
                messageContent = statusUpdate;
                foreach (string friendName in friendNames)
                {
                    Unicast(messageType, messageContent, friendName);
                }
            }
        }

        /// <summary>
        /// The "HandleUpdateProfilePictureRequestEnum" method handles a request to update the user's profile picture.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the new profile picture ID.</param>
        /// <remarks>
        /// This method inserts the new profile picture ID into the database for the current user.
        /// If the insertion is successful, it creates a ProfilePictureUpdate object with the user's nickname and the new profile picture ID.
        /// It then retrieves the list of friends for the current user and sends an update message to each friend with the new profile picture ID.
        /// It also retrieves the list of common chat users for the current user and sends an update message to each chat user who is not a friend.
        /// Finally, it sends a response message to the sender confirming the profile picture update.
        /// </remarks>
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
                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.UpdateProfilePictureResponse_Sender;
                object messageContent = profilePictureId;
                SendMessage(messageType, messageContent);

                messageType = EnumHandler.CommunicationMessageID_Enum.UpdateProfilePictureResponse_ContactReciever;
                messageContent = profilePictureUpdate;
                foreach (string friendName in friendNames)
                {
                    Unicast(messageType, messageContent, friendName);
                }
                messageType = EnumHandler.CommunicationMessageID_Enum.UpdateProfilePictureResponse_ChatUserReciever;
                foreach (string chatUser in chatUsers_NotContants)
                {
                    Unicast(messageType, messageContent, chatUser);
                }
            }
        }

        /// <summary>
        /// The "HandleUserDetailsRequestEnum" method handles a request to retrieve the user's details.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the request details.</param>
        /// <remarks>
        /// This method retrieves the user's profile settings from the database based on the current user's nickname.
        /// It then sends a response message to the sender containing the user's details.
        /// </remarks>
        private void HandleUserDetailsRequestEnum(JsonObject jsonObject)
        {
            JsonClasses.UserDetails userDetails = DataHandler.GetUserProfileSettings(_ClientNick);

            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.UserDetailsResponse;
            object messageContent = userDetails;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleProfileState" method handles the profile state based on a list of communication message IDs.
        /// </summary>
        /// <param name="phaseEnum">The list of communication message IDs representing different phases of profile setup.</param>
        /// <remarks>
        /// This method checks the current state of the user's profile setup based on the provided list of message IDs.
        /// If the user's profile picture is missing, it sets the next phase to set the profile picture.
        /// If the user's status is missing, it sets the next phase to set the user's status.
        /// If the user is not online, it sets the next phase to open the chat.
        /// If any errors occur during the profile setup, it sets the next phase to handle the error.
        /// It then sends a message with the next phase to proceed with the profile setup.
        /// </remarks>
        private void HandleProfileState(List<EnumHandler.CommunicationMessageID_Enum> phaseEnum)
        {
            if (phaseEnum == null || phaseEnum.Count != 4)
            {
                return;
            }
            EnumHandler.CommunicationMessageID_Enum setUserProfilePictrue = phaseEnum[0];
            EnumHandler.CommunicationMessageID_Enum setUserStatus = phaseEnum[1];
            EnumHandler.CommunicationMessageID_Enum openChat = phaseEnum[2];
            EnumHandler.CommunicationMessageID_Enum handleError = phaseEnum[3];

            EnumHandler.CommunicationMessageID_Enum PersonalVerificationAnswersNextPhaseEnum;

            if (!DataHandler.ProfilePictureIsExist(_ClientNick))
            {
                PersonalVerificationAnswersNextPhaseEnum = setUserProfilePictrue;
            }
            else if (!DataHandler.StatusIsExist(_ClientNick))
            {
                PersonalVerificationAnswersNextPhaseEnum = setUserStatus;
            }
            else if (DataHandler.SetUserOnline(_ClientNick) > 0)
            {
                _isOnline = true;
                PersonalVerificationAnswersNextPhaseEnum = openChat;
            }
            else
            {
                PersonalVerificationAnswersNextPhaseEnum = handleError;
            }

            EnumHandler.CommunicationMessageID_Enum messageType = PersonalVerificationAnswersNextPhaseEnum;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleInitialProfileSettingsCheckRequestEnum" method handles a request to check the initial profile settings.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the request details.</param>
        /// <remarks>
        /// This method creates a list of communication message IDs representing the initial profile settings check response phases.
        /// It then calls the HandleProfileState method to handle the profile state based on the list of phases.
        /// </remarks>
        private void HandleInitialProfileSettingsCheckRequestEnum(JsonObject jsonObject) 
        {
            List<EnumHandler.CommunicationMessageID_Enum> initialProfileSettingsCheckResponse = new List<EnumHandler.CommunicationMessageID_Enum>
            {
                EnumHandler.CommunicationMessageID_Enum.InitialProfileSettingsCheckResponse_SetUserProfilePicture,
                EnumHandler.CommunicationMessageID_Enum.InitialProfileSettingsCheckResponse_SetUserStatus,
                EnumHandler.CommunicationMessageID_Enum.InitialProfileSettingsCheckResponse_OpenChat,
                EnumHandler.CommunicationMessageID_Enum.InitialProfileSettingsCheckResponse_HandleError
            };
            HandleProfileState(initialProfileSettingsCheckResponse);
        }

        /// <summary>
        /// The "HandlePersonalVerificationAnswersRequestEnum" method handles a request for personal verification answers.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the personal verification answers.</param>
        /// <remarks>
        /// This method checks the user's verification information against the provided answers.
        /// If the verification is successful, it checks if a password update is needed and handles the profile state accordingly.
        /// If the verification fails, it handles the failed attempt by banning the user from logging in.
        /// </remarks>
        private void HandlePersonalVerificationAnswersRequestEnum(JsonObject jsonObject)
        {
            PersonalVerificationAnswers personalVerificationAnswers = jsonObject.MessageBody as PersonalVerificationAnswers;

            if (DataHandler.CheckUserVerificationInformation(_ClientNick, personalVerificationAnswers))
            {
                _LoginFailedAttempts = new ClientAttemptsState(this, EnumHandler.UserAuthentication_Enum.Login);
                if (PasswordUpdate.IsNeededToUpdatePassword(_ClientNick)) //opens the user the change password mode, he changes the password and if it's possible it automatticly let him enter or he needs to login once again...
                {
                    EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_UpdatePassword;
                    object messageContent = null;
                    SendMessage(messageType, messageContent);
                }
                else
                {
                    List<EnumHandler.CommunicationMessageID_Enum> successfulPersonalVerificationAnswersResponse = new List<EnumHandler.CommunicationMessageID_Enum>
                    {
                        EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_SetUserProfilePicture,
                        EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_SetUserStatus,
                        EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_OpenChat,
                        EnumHandler.CommunicationMessageID_Enum.SuccessfulPersonalVerificationAnswersResponse_HandleError
                    };
                    HandleProfileState(successfulPersonalVerificationAnswersResponse);
                }
            }
            else
            {
                HandleFailedAttempt(_LoginFailedAttempts, EnumHandler.CommunicationMessageID_Enum.LoginBanStart, SetFailedPersonalVerificationAnswers);
            }
        }

        /// <summary>
        /// The "SetFailedPersonalVerificationAnswers" method sends a response for failed personal verification answers.
        /// </summary>
        /// <remarks>
        /// This method sends a message to the client indicating that the personal verification answers were incorrect.
        /// </remarks>
        private void SetFailedPersonalVerificationAnswers()
        {
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.FailedPersonalVerificationAnswersResponse;
            object messageContent = null;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleCaptchaImageAngleRequestEnum" method handles a request related to the captcha image angle.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the captcha image angle request details.</param>
        /// <remarks>
        /// This method checks the angle of the captcha image provided in the request.
        /// If the angle is correct and the captcha rotation attempts are successful, it retrieves user verification questions
        /// and sends them to the client along with the captcha rotation success rate.
        /// If the captcha rotation attempts fail, it handles the ban of the user and logs the event.
        /// If the captcha rotation attempts are not exhausted, it sends details of the captcha rotation image for further attempts.
        /// </remarks>
        private void HandleCaptchaImageAngleRequestEnum(JsonObject jsonObject)
        {
            EnumHandler.CommunicationMessageID_Enum messageType;
            object messageContent;
            double captchaImageAngle = (double)jsonObject.MessageBody;
            captchaRotatingImageHandler.CheckAngle(captchaImageAngle);
            if (captchaRotatingImageHandler.CheckAttempts())
            {
                if (captchaRotatingImageHandler.CheckSuccess())
                {
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
    
                    messageType = EnumHandler.CommunicationMessageID_Enum.SuccessfulCaptchaImageAngleResponse;
                    messageContent = verificationQuestionDetails;
                }
                else
                {
                    if (_captchaRotationImagesAttemptsState == null)
                    {
                        _captchaRotationImagesAttemptsState = new ClientCaptchaRotationImagesAttemptsState(this);
                    }
                    int score = captchaRotatingImageHandler.GetScore();
                    _captchaRotationImagesAttemptsState.HandleBan(score);
                    Logger.LogUserBanStart($"A user has been blocked from the server with the following IP: {_clientIP}.");
                    double banDuration = _captchaRotationImagesAttemptsState.CurrentBanDuration;
                    messageType = EnumHandler.CommunicationMessageID_Enum.LoginBanStart;
                    messageContent = banDuration;
                }
            }
            else
            {
                CaptchaRotationImageDetails captchaRotationImageDetails = GetCaptchaRotationImageDetails();
                messageType = EnumHandler.CommunicationMessageID_Enum.CaptchaImageAngleResponse;
                messageContent = captchaRotationImageDetails;
            }
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "GetCaptchaRotationImageDetails" method retrieves the details of the captcha rotation image along with the success rate.
        /// </summary>
        /// <returns>A <see cref="CaptchaRotationImageDetails"/> object containing the captcha rotation images and success rate.</returns>
        /// <remarks>
        /// This method is used to obtain the captcha rotation images and their corresponding success rate.
        /// It retrieves the current score and attempts from the captcha rotating image handler.
        /// The method then calls the overloaded <see cref="GetCaptchaRotationImageDetails(int, int)"/> method
        /// with the score and attempts as parameters to get the captcha rotation images and success rate.
        /// </remarks>
        private CaptchaRotationImageDetails GetCaptchaRotationImageDetails()
        {
            int score = captchaRotatingImageHandler.GetScore();
            int attempts = captchaRotatingImageHandler.GetAttempts();
            CaptchaRotationImageDetails captchaRotationImageDetails = GetCaptchaRotationImageDetails(score, attempts);
            return captchaRotationImageDetails;
        }

        /// <summary>
        /// The "HandlePastFriendRequestsRequestEnum" method handles a request to retrieve past friend requests for the user.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the request details.</param>
        /// <remarks>
        /// This method retrieves a list of past friend requests for the user from the data handler.
        /// It iterates through each past friend request to check if there is a profile picture available for the friend's username,
        /// and if found, it updates the profile picture in the past friend request object.
        /// Finally, it creates a PastFriendRequests object with the updated list of past friend requests and sends it as a response.
        /// </remarks>
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

            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.PastFriendRequestsResponse;
            object messageContent = pastFriendRequests;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleContactInformationRequestEnum" method handles a request to retrieve contact information for the user's friends.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the request details.</param>
        /// <remarks>
        /// This method retrieves the list of friend names for the user from the data handler.
        /// It then uses the friend names to fetch the contacts' profile information from the data handler.
        /// The method creates a Contacts object with the retrieved information and sends it as a response.
        /// It also sends an "OnlineUpdate" message to each friend to notify them of the user's online status.
        /// </remarks>
        private void HandleContactInformationRequestEnum(JsonObject jsonObject)
        {
            List<string> friendNames = DataHandler.GetFriendList(_ClientNick);
            Contacts contacts = DataHandler.GetFriendsProfileInformation(friendNames);
  
            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.ContactInformationResponse;
            object messageContent = contacts;
            SendMessage(messageType, messageContent);

            messageType = EnumHandler.CommunicationMessageID_Enum.OnlineUpdate;
            messageContent = _ClientNick;
            foreach (string friendName in friendNames)
            {
                Unicast(messageType, messageContent, friendName);
            }
        }

        /// <summary>
        /// The "HandleChatInformationRequestEnum" method handles a request to retrieve chat information for the user.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the request details.</param>
        /// <remarks>
        /// This method retrieves the list of user chats from the chat handler.
        /// It creates a Chats object with the retrieved chat details and sends it as a response.
        /// </remarks>
        private void HandleChatInformationRequestEnum(JsonObject jsonObject)
        {
            List<ChatDetails> userChats = ChatHandler.ChatHandler.GetUserChats(_ClientNick);
            Chats chats = new Chats(userChats);

            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.ChatInformationResponse;
            object messageContent = chats;
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleGroupCreatorRequestEnum" method handles a request to create a new group chat.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the details of the new group chat.</param>
        /// <remarks>
        /// This method extracts the chat name, participants, and profile picture bytes from the JsonObject.
        /// It sets a tagline for the chat and creates an XML file manager for the chat.
        /// If the group chat creation is successful, it adds the chat to the chat handler and sends a response.
        /// </remarks>
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
                GroupChatDetails groupChat = (GroupChatDetails)chat;

                EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.GroupCreatorResponse;
                object messageContent = groupChat;
                SendMessage(messageType, messageContent);
                SendChatMessage(messageType, messageContent, ChatTagLine);
            }
            else
            {
                xmlFileManager.DeleteFile();
            }
        }

        /// <summary>
        /// The "HandleVideoCallRequestEnum" method handles a video call request between two users.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the video call details.</param>
        /// <remarks>
        /// This method creates a list of communication message IDs for handling video call responses.
        /// It then calls the HandleCallRequest method to process the video call request using the provided JsonObject.
        /// </remarks>
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

        /// <summary>
        /// The "HandleAudioCallRequestEnum" method handles an audio call request between two users.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the audio call details.</param>
        /// <remarks>
        /// This method creates a list of communication message IDs for handling audio call responses.
        /// It then calls the HandleCallRequest method to process the audio call request using the provided JsonObject.
        /// </remarks>
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

        /// <summary>
        /// The "HandleCallRequest" method handles a call request between two users.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the call details.</param>
        /// <param name="callResponses">The list of communication message IDs for handling call responses.</param>
        /// <remarks>
        /// This method checks the validity of the call responses list.
        /// It then retrieves the chat details from the provided JsonObject and determines the type of chat (direct or group).
        /// If the chat is a direct chat, it checks if the other user is online and not in another call,
        /// and sends the appropriate messages to both users to establish the call.
        /// </remarks>
        private void HandleCallRequest(JsonObject jsonObject,List<EnumHandler.CommunicationMessageID_Enum> callResponses)
        {
            if (callResponses == null || callResponses.Count != 3)
            {
                return;
            }
            EnumHandler.CommunicationMessageID_Enum messageType;
            object messageContent;

            EnumHandler.CommunicationMessageID_Enum SuccessfulCallResponse_Sender = callResponses[0];
            EnumHandler.CommunicationMessageID_Enum SuccessfulCallResponse_Reciever = callResponses[1];
            EnumHandler.CommunicationMessageID_Enum FailedCallResponse = callResponses[2];

            string chatId = jsonObject.MessageBody as string;
            ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            if (chat is DirectChatDetails directChat)
            {
                string friendName = directChat.GetOtherUserName(_ClientNick);
                if (UserIsOnline(friendName) && !IsUserInCall(friendName) && !IsUserInCall(_ClientNick))
                {
                    messageType = SuccessfulCallResponse_Reciever;
                    messageContent = chatId;
                    Unicast(messageType, messageContent, friendName);

                    _inCall = true;
                    SetUserInCall(friendName, true);

                    messageType = SuccessfulCallResponse_Sender;
                }
                else
                {
                    messageType = FailedCallResponse;
                }
                messageContent = null;
                SendMessage(messageType, messageContent);
            }
        }

        /// <summary>
        /// The "HandleVideoCallAcceptanceRequestEnum" method handles a request to accept a video call.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the video call acceptance details.</param>
        /// <remarks>
        /// This method calls the "HandleCallAcceptanceRequestEnum" method with the appropriate parameters to handle the video call acceptance.
        /// </remarks>
        private void HandleVideoCallAcceptanceRequestEnum(JsonObject jsonObject)
        {
            HandleCallAcceptanceRequestEnum(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallAcceptanceResponse, EnumHandler.CommunicationMessageID_Enum.SuccessfulVideoCallResponse_Reciever, true);
        }

        /// <summary>
        /// The "HandleAudioCallAcceptanceRequestEnum" method handles a request to accept an audio call.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the audio call acceptance details.</param>
        /// <remarks>
        /// This method calls the "HandleCallAcceptanceRequestEnum" method with the appropriate parameters to handle the audio call acceptance.
        /// </remarks>
        private void HandleAudioCallAcceptanceRequestEnum(JsonObject jsonObject)
        {
            HandleCallAcceptanceRequestEnum(jsonObject, EnumHandler.CommunicationMessageID_Enum.AudioCallAcceptanceResponse, EnumHandler.CommunicationMessageID_Enum.SuccessfulAudioCallResponse_Reciever,false);
        }

        /// <summary>
        /// The "HandleCallAcceptanceRequestEnum" method handles a request to accept a call.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the call acceptance details.</param>
        /// <param name="callAcceptanceResponse">The response message ID for a successful call acceptance.</param>
        /// <param name="failedResponse">The response message ID for a failed call acceptance.</param>
        /// <param name="HandleVideo">A boolean indicating whether to handle video calls.</param>
        /// <remarks>
        /// This method attempts to accept the call by sending the acceptance response message to the caller.
        /// If the call acceptance fails, it restarts the call invitation process.
        /// </remarks>
        private void HandleCallAcceptanceRequestEnum(JsonObject jsonObject, EnumHandler.CommunicationMessageID_Enum callAcceptanceResponse, EnumHandler.CommunicationMessageID_Enum failedResponse, bool HandleVideo)
        {
            string chatId = jsonObject.MessageBody as string;
            ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            try
            {
                DirectChatDetails directChat = (DirectChatDetails)chat;
                string friendName = directChat.GetOtherUserName(_ClientNick);
                if (IsUserInCall(friendName) && IsUserInCall(_ClientNick))
                {
                    EnumHandler.CommunicationMessageID_Enum messageType = callAcceptanceResponse;
                    object messageContent = chatId;
                    SendMessage(messageType, messageContent);
                    Unicast(messageType, messageContent, friendName);

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

        /// <summary>
        /// The "HandleVideoCallDenialRequestEnum" method handles a request to deny a video call.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the video call denial details.</param>
        /// <remarks>
        /// This method delegates the call denial handling to the "HandleCallDenialRequestEnum" method
        /// with the appropriate response message IDs for video call denial.
        /// </remarks>
        private void HandleVideoCallDenialRequestEnum(JsonObject jsonObject)
        {
            HandleCallDenialRequestEnum(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallDenialResponse, EnumHandler.CommunicationMessageID_Enum.SuccessfulVideoCallResponse_Reciever);
        }

        /// <summary>
        /// The "HandleAudioCallDenialRequestEnum" method handles a request to deny an audio call.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the audio call denial details.</param>
        /// <remarks>
        /// This method delegates the call denial handling to the "HandleCallDenialRequestEnum" method
        /// with the appropriate response message IDs for audio call denial.
        /// </remarks>
        private void HandleAudioCallDenialRequestEnum(JsonObject jsonObject)
        {
            HandleCallDenialRequestEnum(jsonObject, EnumHandler.CommunicationMessageID_Enum.AudioCallDenialResponse,EnumHandler.CommunicationMessageID_Enum.SuccessfulAudioCallResponse_Reciever);
        }

        /// <summary>
        /// The "HandleCallDenialRequestEnum" method handles a request to deny a call.
        /// </summary>
        /// <param name="jsonObject">The JsonObject containing the call denial details.</param>
        /// <param name="callDenialResponse">The response message ID for a successful call denial.</param>
        /// <param name="failedResponse">The response message ID for a failed call denial.</param>
        /// <remarks>
        /// This method attempts to deny the call by sending the denial response message to the caller.
        /// If the call denial fails, it restarts the call invitation process.
        /// </remarks>
        private void HandleCallDenialRequestEnum(JsonObject jsonObject, EnumHandler.CommunicationMessageID_Enum callDenialResponse, EnumHandler.CommunicationMessageID_Enum failedResponse)
        {
            string chatId = jsonObject.MessageBody as string;
            ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            try
            {
                DirectChatDetails directChat = (DirectChatDetails)chat;
                string friendName = directChat.GetOtherUserName(_ClientNick);
                if (IsUserInCall(friendName) && IsUserInCall(_ClientNick))
                {
                    EnumHandler.CommunicationMessageID_Enum messageType = callDenialResponse;
                    object messageContent = null;
                    Unicast(messageType, messageContent, friendName);

                    _inCall = false;
                    SetUserInCall(friendName, false);
                }
            }
            catch
            {
                RestartCallInvitation(chatId, failedResponse);
            }
        }

        /// <summary>
        /// The "HandleMessageHistoryRequestEnum" method is responsible for processing a request to retrieve the message history for a specific chat.
        /// </summary>
        /// <param name="jsonObject">A JsonObject containing the chat ID for which the message history is requested.</param>
        /// <remarks>
        /// This method reads the message history from the chat's XML file, converts the message data into a format suitable for transmission,
        /// and constructs a list of messages to be sent back as a response. It first retrieves the chat's XML file manager based on the provided
        /// chat ID, then reads the XML file to obtain a list of MessageData objects representing the chat's message history. For each MessageData
        /// object, it parses the message date and time, determines the message content type (text, image, or deleted message), and creates a
        /// JsonClasses.Message object with the appropriate content. These JsonClasses.Message objects are then aggregated into a MessageHistory
        /// object, which is sent back as the response to the message history request.
        /// </remarks>
        private void HandleMessageHistoryRequestEnum(JsonObject jsonObject)
        {
            string chatId = jsonObject.MessageBody as string;
            XmlFileManager xmlFileManager = ChatHandler.ChatHandler.ChatFileManagers[chatId];
            List<MessageData> messageDatas =  xmlFileManager.ReadChatXml();
            List<JsonClasses.Message> messages = new List<JsonClasses.Message>();
            string messageSenderName;
            object messageContentValue = null;
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
                        messageContentValue = messageData.Content;
                        break;
                    case "Image":
                        if (!string.IsNullOrEmpty(messageData.Content))
                        {
                            // Convert string to byte array
                            byte[] imageData = Convert.FromBase64String(messageData.Content);
                            ImageContent imageContent = new ImageContent(imageData);
                            messageContentValue = imageContent;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid content type for Image.");
                        }
                        break;
                    case "DeletedMessage":
                        messageContentValue = null;
                        break;
                }

                messageSenderName = messageData.Sender;
                JsonClasses.Message message = new JsonClasses.Message(messageSenderName, chatId, messageContentValue, messageDateAndTime);
                messages.Add(message);
            }

            EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.MessageHistoryResponse;
            object messageContent;
            if (messages.Count == 0)
            {
                messageContent = chatId;
            }
            else
            {
                MessageHistory messageHistory = new MessageHistory(messages);
                messageContent = messageHistory;

            }
            SendMessage(messageType, messageContent);
        }

        /// <summary>
        /// The "HandleVideoCallMuteRequestEnum" method handles a request to mute a video call.
        /// </summary>
        /// <param name="jsonObject">A JsonObject containing the request information.</param>
        /// <remarks>
        /// This method forwards the request to the "HandleVideoCallRequest" method with the appropriate video call response message ID
        /// to handle the mute action in the video call.
        /// </remarks>
        private void HandleVideoCallMuteRequestEnum(JsonObject jsonObject)
        {
            HandleVideoCallRequest(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallMuteResponse);
        }

        /// <summary>
        /// The "HandleVideoCallUnmuteRequestEnum" method handles a request to unmute a video call.
        /// </summary>
        /// <param name="jsonObject">A JsonObject containing the request information.</param>
        /// <remarks>
        /// This method forwards the request to the "HandleVideoCallRequest" method with the appropriate video call response message ID
        /// to handle the unmute action in the video call.
        /// </remarks>
        private void HandleVideoCallUnmuteRequestEnum(JsonObject jsonObject)
        {
            HandleVideoCallRequest(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallUnmuteResponse);
        }

        /// <summary>
        /// The "HandleVideoCallCameraOnRequestEnum" method handles a request to turn on the camera in a video call.
        /// </summary>
        /// <param name="jsonObject">A JsonObject containing the request information.</param>
        /// <remarks>
        /// This method forwards the request to the "HandleVideoCallRequest" method with the appropriate video call response message ID
        /// to handle turning on the camera in the video call.
        /// </remarks>
        private void HandleVideoCallCameraOnRequestEnum(JsonObject jsonObject)
        {   
            HandleVideoCallRequest(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallCameraOnResponse);
        }

        /// <summary>
        /// The "HandleVideoCallCameraOffRequestEnum" method handles a request to turn off the camera in a video call.
        /// </summary>
        /// <param name="jsonObject">A JsonObject containing the request information.</param>
        /// <remarks>
        /// This method forwards the request to the "HandleVideoCallRequest" method with the appropriate video call response message ID
        /// to handle turning off the camera in the video call.
        /// </remarks>
        private void HandleVideoCallCameraOffRequestEnum(JsonObject jsonObject)
        {
            HandleVideoCallRequest(jsonObject, EnumHandler.CommunicationMessageID_Enum.VideoCallCameraOffResponse);
        }

        /// <summary>
        /// The "HandleEndVideoCallRequestEnum" method handles a request to end a video call.
        /// </summary>
        /// <param name="jsonObject">A JsonObject containing the request information, including the chat ID and socket ports.</param>
        /// <remarks>
        /// This method processes the request to end a video call between two clients. It retrieves the chat details based on the provided chat ID
        /// and checks if both clients are still in the call. If both clients are in the call, it sends end video call messages to both clients,
        /// removes the endpoints associated with the audio and video streams, and updates the call status for both clients. If the chat details
        /// cannot be retrieved or one or both clients are not in the call, the method does nothing.
        /// </remarks>
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
                if (IsUserInCall(friendName) && IsUserInCall(_ClientNick))
                {
                    EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.EndVideoCallResponse_Reciever;
                    object messageContent = null;
                    Unicast(messageType, messageContent, friendName);

                    messageType = EnumHandler.CommunicationMessageID_Enum.EndVideoCallResponse_Sender;
                    SendMessage(messageType, messageContent);
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

        /// <summary>
        /// The "HandleEndAudioCallRequestEnum" method handles a request to end an audio call.
        /// </summary>
        /// <param name="jsonObject">A JsonObject containing the request information, including the chat ID and socket port.</param>
        /// <remarks>
        /// This method processes the request to end an audio call between two clients. It retrieves the chat details based on the provided chat ID
        /// and checks if both clients are still in the call. If both clients are in the call, it sends end audio call messages to both clients,
        /// removes the endpoint associated with the audio stream, and updates the call status for both clients. If the chat details cannot be
        /// retrieved or one or both clients are not in the call, the method does nothing.
        /// </remarks>
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
                if (IsUserInCall(friendName) && IsUserInCall(_ClientNick))
                {
                    EnumHandler.CommunicationMessageID_Enum messageType = EnumHandler.CommunicationMessageID_Enum.EndAudioCallResponse_Reciever;
                    object messageContent = null;
                    Unicast(messageType, messageContent, friendName);

                    messageType = EnumHandler.CommunicationMessageID_Enum.EndAudioCallResponse_Sender;
                    SendMessage(messageType, messageContent);

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

        /// <summary>
        /// The "HandleVideoCallRequest" method is responsible for processing a video call request.
        /// </summary>
        /// <param name="jsonObject">A JsonObject containing the request information.</param>
        /// <param name="videoCallRequest">The type of video call request to handle.</param>
        /// <remarks>
        /// This method retrieves the chat ID from the JsonObject and attempts to find the corresponding chat in the chat handler.
        /// If the chat is found and both users are in a call, it sends a unicast message to the friend's client with the specified video call request.
        /// </remarks>
        private void HandleVideoCallRequest(JsonObject jsonObject, EnumHandler.CommunicationMessageID_Enum videoCallRequest)
        {
            string chatId = jsonObject.MessageBody as string;
            ChatDetails chat = ChatHandler.ChatHandler.AllChats[chatId];
            try
            {
                DirectChatDetails directChat = (DirectChatDetails)chat;
                string friendName = directChat.GetOtherUserName(_ClientNick);
                if (IsUserInCall(friendName) && IsUserInCall(_ClientNick))
                {
                    EnumHandler.CommunicationMessageID_Enum messageType = videoCallRequest;
                    object messageContent = null;
                    Unicast(messageType, messageContent, friendName);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// The "RestartCallInvitation" method restarts a call invitation by sending a message with the specified chat ID.
        /// </summary>
        /// <param name="chatId">The chat ID associated with the call invitation.</param>
        /// <param name="restartCallInvitation">The message ID for restarting the call invitation.</param>
        /// <remarks>
        /// This method sends a message to restart a call invitation with the specified chat ID. The message is sent using the specified message ID.
        /// </remarks>
        private void RestartCallInvitation(string chatId, EnumHandler.CommunicationMessageID_Enum restartCallInvitation)
        {
            EnumHandler.CommunicationMessageID_Enum messageType = restartCallInvitation;
            object messageContent = chatId;
            SendMessage(messageType, messageContent);
        }

        #endregion

        #region ReceiveMessage Methods

        /// <summary>
        /// The "ReceiveMessageLength" method handles the reception of the message length from the client.
        /// </summary>
        /// <param name="ar">The result of the asynchronous operation.</param>
        /// <remarks>
        /// This method is called when the server receives the length of the message from the client. It reads the length of the message
        /// from the client's stream and prepares to receive the actual message by calling the "ReceiveMessage" method.
        /// If the client has disconnected, it calls the "HandleDisconnectEnum" method. If an exception occurs during the operation,
        /// the method removes the client from the list of active clients and logs the event.
        /// </remarks>
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
                    HandleDisconnectEnum();
                }
            }  
        }

        /// <summary>
        /// The "ReceiveMessage" method handles the reception of messages from the client.
        /// </summary>
        /// <param name="ar">The result of the asynchronous operation.</param>
        /// <remarks>
        /// This method is called when the server receives a message from the client. It reads the message from the client's stream,
        /// decrypts it if necessary, and processes it based on its type. The method first checks if the client is still connected,
        /// and if not, it removes the client from the list of active clients. It then reads the message length and creates a new byte
        /// array to hold the combined data. The method continues to read from the client's stream until it has received the entire message.
        /// It decrypts the message if it is an encrypted message and then deserializes it into a JsonObject. Based on the message type,
        /// the method calls the appropriate handler method to process the message. Finally, the method resets the dataHistory array and
        /// prepares to receive the next message length from the client.
        /// </remarks>
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
                                actualMessage = Encryption.AESServiceProvider.DecryptData(SymmetricKey, actualMessage);
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
                        if (_isClientConnected)
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
                }
            }
        }

        #endregion

        #region SendMessage Methods

        /// <summary>
        /// The "SendMessage" method sends a message to the client.
        /// </summary>
        /// <param name="messageType">The type of message being sent.</param>
        /// <param name="messageContent">The content of the message.</param>
        /// <param name="needEncryption">A flag indicating whether the message should be encrypted before sending (default is true).</param>
        /// <remarks>
        /// This method sends a message to the client over the network stream. It first creates a JSON string from the message type
        /// and content using the JsonHandler class. If encryption is required, the JSON string is encrypted using the Encryption class
        /// and converted to bytes using UTF-8 encoding. The method then constructs a byte array containing a signal byte (indicating
        /// whether the message is encrypted) followed by the message bytes. The total message length is prefixed to the message bytes
        /// and sent in chunks of the specified buffer size. If the message is larger than the buffer size, it is sent in multiple parts.
        /// </remarks>
        public void SendMessage(EnumHandler.CommunicationMessageID_Enum messageType, object messageContent, bool needEncryption = true)
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
                string jsonMessage = JsonClasses.JsonHandler.JsonHandler.GetJsonStringFromJsonData(messageType, messageContent);

                byte signal = needEncryption ? (byte)1 : (byte)0;

                if (needEncryption)
                {
                    jsonMessage = Encryption.AESServiceProvider.EncryptData(SymmetricKey, jsonMessage);
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
                    bytesToSend = new byte[0];
                    buffer = new byte[0];
                    length = new byte[0];
                    prefixedBuffer = new byte[0];

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
                        byte[] newTotalBytesToSend = new byte[totalBytesToSend.Length - (bufferSize - 8)];

                        Array.Copy(totalBytesToSend, bufferSize - 8, newTotalBytesToSend, 0, newTotalBytesToSend.Length); // to get a fixed size of the prefix to the message
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
        }

        /// <summary>
        /// The "Unicast" method sends a message to a specific client identified by their user ID.
        /// </summary>
        /// <param name="messageType">The type of message being sent.</param>
        /// <param name="messageContent">The content of the message.</param>
        /// <param name="UserID">The user ID of the client to send the message to.</param>
        /// <param name="needEncryption">A flag indicating whether the message should be encrypted before sending (default is true).</param>
        /// <remarks>
        /// This method iterates through all connected clients and sends the message to the client with the specified user ID.
        /// If the client is found and is online, the message is sent using the SendMessage method of the Client class.
        /// </remarks>
        public void Unicast(EnumHandler.CommunicationMessageID_Enum messageType, object messageContent, string UserID, bool needEncryption = true)
        {
            Client client;
            foreach (DictionaryEntry c in AllClients)
            {
                client = ((Client)(c.Value));
                if (client._ClientNick == UserID && client._isOnline)
                {
                    ((Client)(c.Value)).SendMessage(messageType,messageContent, needEncryption);
                }
            }
        }

        /// <summary>
        /// The "SendChatMessage" method sends a chat message to all participants in a chat.
        /// </summary>
        /// <param name="messageType">The type of message being sent.</param>
        /// <param name="messageContent">The content of the message.</param>
        /// <param name="chatId">The ID of the chat to send the message to.</param>
        /// <param name="needEncryption">A flag indicating whether the message should be encrypted before sending (default is true).</param>
        /// <remarks>
        /// This method retrieves the chat details for the specified chat ID from the ChatHandler.
        /// If the client is a participant in the chat, the method iterates through all chat participants
        /// and sends the message to each participant (excluding the client itself) if they are online.
        /// The message is sent using the Unicast method with the specified message type, content, and participant name.
        /// </remarks>
        public void SendChatMessage(EnumHandler.CommunicationMessageID_Enum messageType, object messageContent, string chatId, bool needEncryption = true)
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
                        Unicast(messageType, messageContent, chatParticipantName, needEncryption);
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The "GetCaptchaRotationImageDetails" method retrieves the details of the captcha rotation image along with the success rate.
        /// </summary>
        /// <param name="score">An integer representing the success rate of the captcha rotation attempts.</param>
        /// <param name="attempts">An integer indicating the number of attempts made to rotate the captcha image.</param>
        /// <returns>A <see cref="CaptchaRotationImageDetails"/> object containing the captcha rotation images and success rate.</returns>
        /// <remarks>
        /// This method is used to obtain the captcha rotation images and their corresponding success rate.
        /// It creates a new <see cref="CaptchaRotationSuccessRate"/> object with the provided score and attempts.
        /// The method then returns a <see cref="CaptchaRotationImageDetails"/> object containing the captcha rotation images and success rate.
        /// </remarks>
        public CaptchaRotationImageDetails GetCaptchaRotationImageDetails(int score, int attempts)
        {
            CaptchaRotationImages captchaRotationImages = captchaRotatingImageHandler.GetCaptchaRotationImages();
            CaptchaRotationSuccessRate captchaRotationSuccessRate = new CaptchaRotationSuccessRate(score, attempts);
            CaptchaRotationImageDetails captchaRotationImageDetails = new CaptchaRotationImageDetails(captchaRotationImages, captchaRotationSuccessRate);
            return captchaRotationImageDetails;
        }


        /// <summary>
        /// The "UserIsConnected" method checks if a user with the specified username is connected to the server.
        /// </summary>
        /// <param name="username">The username of the user to check.</param>
        /// <returns>True if the user is connected, otherwise false.</returns>
        /// <remarks>
        /// This method iterates through all clients connected to the server and checks if any client's username matches
        /// the specified username. It returns true if a match is found, indicating that the user is connected.
        /// </remarks>
        public bool UserIsConnected(string username)
        {
            foreach (DictionaryEntry c in AllClients)
            {
                if (((Client)(c.Value))._ClientNick == username) 
                    return true;
            }
            return false;
        }

        /// <summary>
        /// The "UserIsOnline" method checks if a user with the specified username is both connected and online on the server.
        /// </summary>
        /// <param name="username">The username of the user to check.</param>
        /// <returns>True if the user is both connected and online, otherwise false.</returns>
        /// <remarks>
        /// This method iterates through all clients connected to the server and checks if any client's username matches
        /// the specified username and the client is marked as online. It returns true if a match is found, indicating that
        /// the user is both connected and online.
        /// </remarks>
        public bool UserIsOnline(string username)
        {
            Client client;
            foreach (DictionaryEntry c in AllClients)
            {
                client = (Client)(c.Value);
                if (client._ClientNick == username && client._isOnline) 
                    return true;
            }
            return false;
        }

        /// <summary>
        /// The "IsUserInCall" method checks if a user with the specified username is currently in a call.
        /// </summary>
        /// <param name="username">The username of the user to check.</param>
        /// <returns>True if the user is currently in a call, otherwise false.</returns>
        /// <remarks>
        /// This method iterates through all clients connected to the server and checks if any client's username matches
        /// the specified username and the client is marked as in a call. It returns true if a match is found, indicating
        /// that the user is currently in a call.
        /// </remarks>
        public bool IsUserInCall(string username)
        {
            Client client;
            foreach (DictionaryEntry c in AllClients)
            {
                client = (Client)(c.Value);
                if (client._ClientNick == username && client._inCall) 
                    return true;
            }
            return false;
        }

        /// <summary>
        /// The "SetUserInCall" method sets the in-call status of a user with the specified username.
        /// </summary>
        /// <param name="username">The username of the user whose in-call status to set.</param>
        /// <param name="value">The value indicating whether the user is in a call (true) or not (false).</param>
        /// <remarks>
        /// This method iterates through all clients connected to the server and sets the in-call status of the client
        /// with the specified username to the specified value. If no client with the specified username is found, no
        /// action is taken.
        /// </remarks>
        public void SetUserInCall(string username, bool value)
        {
            Client client;
            foreach (DictionaryEntry c in AllClients)
            {
                client = (Client)(c.Value);
                if (client._ClientNick == username) 
                    client._inCall = value;
            }
        }

        /// <summary>
        /// The "GetUserEndPoint" method retrieves the IP end point of a user with the specified username.
        /// </summary>
        /// <param name="username">The username of the user whose IP end point to retrieve.</param>
        /// <returns>
        /// The IP end point of the user with the specified username, or null if no user with that username is found.
        /// </returns>
        /// <remarks>
        /// This method iterates through all clients connected to the server and retrieves the IP end point of the client
        /// with the specified username. If no client with the specified username is found, it returns null.
        /// </remarks>
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

        /// <summary>
        /// The "GetIpAddress" method returns the IP address of the client.
        /// </summary>
        /// <returns>The IP address of the client.</returns>
        public string GetIpAddress()
        {
            return _clientIP;
        }

        #endregion
    }
}
