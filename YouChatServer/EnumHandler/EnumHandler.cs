using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    /// <summary>
    /// The "EnumHandler" class contains enums for various purposes, such as user authentication, SMTP message types, and communication message IDs.
    /// </summary>
    public static class EnumHandler
    {
        #region Enums

        /// <summary>
        /// Enum representing different user authentication scenarios.
        /// </summary>
        public enum UserAuthentication_Enum
        {
            /// <summary>
            /// Represents the login authentication scenario.
            /// </summary>
            Login,

            /// <summary>
            /// Represents the registration authentication scenario.
            /// </summary>
            Registration,

            /// <summary>
            /// Represents the password update authentication scenario.
            /// </summary>
            PasswordUpdate,

            /// <summary>
            /// Represents the password restart authentication scenario.
            /// </summary>
            PasswordRestart
        }

        /// <summary>
        /// Enum representing different types of SMTP messages.
        /// </summary>
        public enum SmtpMessageType_Enum
        {
            /// <summary>
            /// Represents a registration message.
            /// </summary>
            RegistrationMessage,

            /// <summary>
            /// Represents a login message.
            /// </summary>
            LoginMessage,

            /// <summary>
            /// Represents a password renewal message.
            /// </summary>
            PasswordRenewalMessage
        }

        /// <summary>
        /// Enum representing different communication message IDs.
        /// </summary>
        public enum CommunicationMessageID_Enum
        {
            /// <summary>
            /// Represents a login request.
            /// </summary>
            loginRequest,

            /// <summary>
            /// Represents a login request for SMTP login message.
            /// </summary>
            loginRequest_SmtpLoginMessage,

            /// <summary>
            /// Represents a login response for SMTP login message.
            /// </summary>
            loginResponse_SmtpLoginMessage,

            /// <summary>
            /// Represents a login response for a failed login attempt.
            /// </summary>
            loginResponse_FailedLogin,

            /// <summary>
            /// Represents a login request for SMTP login code.
            /// </summary>
            LoginRequest_SmtpLoginCode,

            /// <summary>
            /// Represents a login response for a successful SMTP login code.
            /// </summary>
            LoginResponse_SuccessfulSmtpLoginCode,

            /// <summary>
            /// Represents a login response for a failed SMTP login code.
            /// </summary>
            LoginResponse_FailedSmtpLoginCode,

            /// <summary>
            /// Represents a registration request for SMTP registration message.
            /// </summary>
            RegistrationRequest_SmtpRegistrationMessage,

            /// <summary>
            /// Represents a registration response for SMTP registration message.
            /// </summary>
            RegistrationResponse_SmtpRegistrationMessage,

            /// <summary>
            /// Represents a registration request for SMTP registration code.
            /// </summary>
            RegistrationRequest_SmtpRegistrationCode,

            /// <summary>
            /// Represents a registration response for a successful SMTP registration code.
            /// </summary>
            RegistrationResponse_SuccessfulSmtpRegistrationCode,

            /// <summary>
            /// Represents a registration response for a failed SMTP registration code.
            /// </summary>
            RegistrationResponse_FailedSmtpRegistrationCode,

            /// <summary>
            /// Represents a registration request for general registration.
            /// </summary>
            RegistrationRequest_Registration,

            /// <summary>
            /// Represents a registration response for a successful general registration.
            /// </summary>
            RegistrationResponse_SuccessfulRegistration,

            /// <summary>
            /// Represents a registration response for a failed general registration.
            /// </summary>
            RegistrationResponse_FailedRegistration,

            /// <summary>
            /// Represents a message containing the encryption client's public key.
            /// </summary>
            EncryptionClientPublicKeySender,

            /// <summary>
            /// Represents a message containing the encryption server's public key and symmetric key receiver.
            /// </summary>
            EncryptionServerPublicKeyAndSymmetricKeyReciever,

            /// <summary>
            /// Represents a message containing the encryption server's public key receiver.
            /// </summary>
            EncryptionServerPublicKeyReciever,

            /// <summary>
            /// Represents a message containing the encryption symmetric key receiver.
            /// </summary>
            EncryptionSymmetricKeyReciever,

            /// <summary>
            /// Represents a message requesting encryption key renewal.
            /// </summary>
            EncryptionRenewKeys,

            /// <summary>
            /// Represents a friend request sender message.
            /// </summary>
            FriendRequestSender,

            /// <summary>
            /// Represents a friend request receiver message.
            /// </summary>
            FriendRequestReciever,

            /// <summary>
            /// Represents a friend request response sender message.
            /// </summary>
            FriendRequestResponseSender,

            /// <summary>
            /// Represents a friend request response receiver message.
            /// </summary>
            FriendRequestResponseReciever,

            /// <summary>
            /// Represents a request to upload a profile picture.
            /// </summary>
            UploadProfilePictureRequest,

            /// <summary>
            /// Represents a response to an upload profile picture request.
            /// </summary>
            UploadProfilePictureResponse,

            /// <summary>
            /// Represents a request to upload a status.
            /// </summary>
            UploadStatusRequest,

            /// <summary>
            /// Represents a response to an upload status request.
            /// </summary>
            UploadStatusResponse,

            /// <summary>
            /// Represents a request for a CAPTCHA image.
            /// </summary>
            CaptchaImageRequest,

            /// <summary>
            /// Represents a response containing a CAPTCHA image.
            /// </summary>
            CaptchaImageResponse,

            /// <summary>
            /// Represents a request for a CAPTCHA code.
            /// </summary>
            CaptchaCodeRequest,

            /// <summary>
            /// Represents a successful response to a CAPTCHA code request.
            /// </summary>
            SuccessfulCaptchaCodeResponse,

            /// <summary>
            /// Represents a failed response to a CAPTCHA code request.
            /// </summary>
            FailedCaptchaCodeResponse,

            /// <summary>
            /// Represents a request for a CAPTCHA image angle.
            /// </summary>
            CaptchaImageAngleRequest,

            /// <summary>
            /// Represents a response containing a CAPTCHA image angle.
            /// </summary>
            CaptchaImageAngleResponse,

            /// <summary>
            /// Represents a successful response to a CAPTCHA image angle request.
            /// </summary>
            SuccessfulCaptchaImageAngleResponse,

            /// <summary>
            /// Represents a failed response to a CAPTCHA image angle request.
            /// </summary>
            FailedCaptchaImageAngleResponse,

            /// <summary>
            /// Represents a request for personal verification answers.
            /// </summary>
            PersonalVerificationAnswersRequest,

            /// <summary>
            /// Represents a successful response to personal verification answers for updating password.
            /// </summary>
            SuccessfulPersonalVerificationAnswersResponse_UpdatePassword,

            /// <summary>
            /// Represents a successful response to personal verification answers for setting user profile picture.
            /// </summary>
            SuccessfulPersonalVerificationAnswersResponse_SetUserProfilePicture,

            /// <summary>
            /// Represents a successful response to personal verification answers for setting user status.
            /// </summary>
            SuccessfulPersonalVerificationAnswersResponse_SetUserStatus,

            /// <summary>
            /// Represents a successful response to personal verification answers for opening chat.
            /// </summary>
            SuccessfulPersonalVerificationAnswersResponse_OpenChat,

            /// <summary>
            /// Represents a successful response to personal verification answers for handling an error.
            /// </summary>
            SuccessfulPersonalVerificationAnswersResponse_HandleError,

            /// <summary>
            /// Represents a failed response to personal verification answers.
            /// </summary>
            FailedPersonalVerificationAnswersResponse,

            /// <summary>
            /// Represents a message to disconnect.
            /// </summary>
            Disconnect,

            /// <summary>
            /// Represents a request to send a message.
            /// </summary>
            SendMessageRequest,

            /// <summary>
            /// Represents a response to a send message request.
            /// </summary>
            SendMessageResponse,

            /// <summary>
            /// Represents a request to establish a UDP audio connection.
            /// </summary>
            UdpAudioConnectionRequest,

            /// <summary>
            /// Represents a response to a UDP audio connection request.
            /// </summary>
            UdpAudioConnectionResponse,

            /// <summary>
            /// Represents a request to establish a UDP video connection.
            /// </summary>
            UdpVideoConnectionRequest,

            /// <summary>
            /// Represents a response to a UDP video connection request.
            /// </summary>
            UdpVideoConnectionResponse,

            /// <summary>
            /// Represents a request to update the password.
            /// </summary>
            PasswordUpdateRequest,

            /// <summary>
            /// Represents a successful response to a password update request.
            /// </summary>
            SuccessfulPasswordUpdateResponse,

            /// <summary>
            /// Represents a failed response to a password update request due to unmatched details.
            /// </summary>
            FailedPasswordUpdateResponse_UnmatchedDetails,

            /// <summary>
            /// Represents a failed response to a password update request due to an existing password.
            /// </summary>
            FailedPasswordUpdateResponse_PasswordExist,

            /// <summary>
            /// Represents an error response to a password update request.
            /// </summary>
            ErrorHandlePasswordUpdateResponse,

            /// <summary>
            /// Represents a request to check initial profile settings.
            /// </summary>
            InitialProfileSettingsCheckRequest,

            /// <summary>
            /// Represents a response to an initial profile settings check request for setting user profile picture.
            /// </summary>
            InitialProfileSettingsCheckResponse_SetUserProfilePicture,

            /// <summary>
            /// Represents a response to an initial profile settings check request for setting user status.
            /// </summary>
            InitialProfileSettingsCheckResponse_SetUserStatus,

            /// <summary>
            /// Represents a response to an initial profile settings check request for opening chat.
            /// </summary>
            InitialProfileSettingsCheckResponse_OpenChat,

            /// <summary>
            /// Represents a response to an initial profile settings check request for handling an error.
            /// </summary>
            InitialProfileSettingsCheckResponse_HandleError,

            /// <summary>
            /// Represents a request for user details.
            /// </summary>
            UserDetailsRequest,

            /// <summary>
            /// Represents a response containing user details.
            /// </summary>
            UserDetailsResponse,

            /// <summary>
            /// Represents a request to change chat settings.
            /// </summary>
            ChatSettingsChangeRequest,

            /// <summary>
            /// Represents a response to a chat settings change request.
            /// </summary>
            ChatSettingsChangeResponse,

            /// <summary>
            /// Represents the start of a login ban.
            /// </summary>
            LoginBanStart,

            /// <summary>
            /// Represents the end of a login ban.
            /// </summary>
            LoginBanFinish,

            /// <summary>
            /// Represents the start of a registration ban.
            /// </summary>
            RegistrationBanStart,

            /// <summary>
            /// Represents the end of a registration ban.
            /// </summary>
            RegistrationBanFinish,

            /// <summary>
            /// Represents the start of a reset password ban.
            /// </summary>
            ResetPasswordBanStart,

            /// <summary>
            /// Represents the end of a reset password ban.
            /// </summary>
            ResetPasswordBanFinish,

            /// <summary>
            /// Represents the start of a password update ban.
            /// </summary>
            PasswordUpdateBanStart,

            /// <summary>
            /// Represents the end of a password update ban.
            /// </summary>
            PasswordUpdateBanFinish,

            /// <summary>
            /// Represents a request to reset the password.
            /// </summary>
            ResetPasswordRequest,

            /// <summary>
            /// Represents a successful response to a reset password request.
            /// </summary>
            SuccessfulResetPasswordResponse,

            /// <summary>
            /// Represents a failed response to a reset password request.
            /// </summary>
            FailedResetPasswordResponse,

            /// <summary>
            /// Represents a reset password request for SMTP message.
            /// </summary>
            ResetPasswordRequest_SmtpMessage,

            /// <summary>
            /// Represents a reset password response for SMTP message.
            /// </summary>
            ResetPasswordResponse_SmtpMessage,

            /// <summary>
            /// Represents a reset password request for SMTP code.
            /// </summary>
            ResetPasswordRequest_SmtpCode,

            /// <summary>
            /// Represents a successful response to a reset password request for SMTP code.
            /// </summary>
            SuccessfulResetPasswordResponse_SmtpCode,

            /// <summary>
            /// Represents a failed response to a reset password request for SMTP code.
            /// </summary>
            FailedResetPasswordResponse_SmtpCode,

            /// <summary>
            /// Represents a request for past friend requests.
            /// </summary>
            PastFriendRequestsRequest,

            /// <summary>
            /// Represents a response containing past friend requests.
            /// </summary>
            PastFriendRequestsResponse,

            /// <summary>
            /// Represents a request for a password renewal message.
            /// </summary>
            PasswordRenewalMessageRequest,

            /// <summary>
            /// Represents a successful response to a password renewal message request.
            /// </summary>
            SuccessfulRenewalMessageResponse,

            /// <summary>
            /// Represents a failed response to a password renewal message request.
            /// </summary>
            FailedRenewalMessageResponse,

            /// <summary>
            /// Represents an error response to a password renewal message request.
            /// </summary>
            ErrorHandleRenewalMessageResponse,

            /// <summary>
            /// Represents a request for chat and contact details.
            /// </summary>
            ChatAndContactDetailsRequest,

            /// <summary>
            /// Represents a response containing contact details.
            /// </summary>
            ChatAndContactDetailsResponse_Contacts,

            /// <summary>
            /// Represents a response containing chat details.
            /// </summary>
            ChatAndContactDetailsResponse_Chats,

            /// <summary>
            /// Represents a request for contact information.
            /// </summary>
            ContactInformationRequest,

            /// <summary>
            /// Represents a response containing contact information.
            /// </summary>
            ContactInformationResponse,

            /// <summary>
            /// Represents a request for chat information.
            /// </summary>
            ChatInformationRequest,

            /// <summary>
            /// Represents a response containing chat information.
            /// </summary>
            ChatInformationResponse,

            /// <summary>
            /// Represents a request for a group creator.
            /// </summary>
            GroupCreatorRequest,

            /// <summary>
            /// Represents a response containing a group creator.
            /// </summary>
            GroupCreatorResponse,

            /// <summary>
            /// Represents a request for a video call.
            /// </summary>
            VideoCallRequest,

            /// <summary>
            /// Represents a successful response to a video call request for the sender.
            /// </summary>
            SuccessfulVideoCallResponse_Sender,

            /// <summary>
            /// Represents a successful response to a video call request for the receiver.
            /// </summary>
            SuccessfulVideoCallResponse_Reciever,

            /// <summary>
            /// Represents a failed response to a video call request.
            /// </summary>
            FailedVideoCallResponse,

            /// <summary>
            /// Represents a request for accepting a video call.
            /// </summary>
            VideoCallAcceptanceRequest,

            /// <summary>
            /// Represents a request for denying a video call.
            /// </summary>
            VideoCallDenialRequest,

            /// <summary>
            /// Represents a response to accepting a video call.
            /// </summary>
            VideoCallAcceptanceResponse,

            /// <summary>
            /// Represents a response to denying a video call.
            /// </summary>
            VideoCallDenialResponse,

            /// <summary>
            /// Represents a request to mute a video call.
            /// </summary>
            VideoCallMuteRequest,

            /// <summary>
            /// Represents a response to muting a video call.
            /// </summary>
            VideoCallMuteResponse,

            /// <summary>
            /// Represents a request to unmute a video call.
            /// </summary>
            VideoCallUnmuteRequest,

            /// <summary>
            /// Represents a response to unmuting a video call.
            /// </summary>
            VideoCallUnmuteResponse,

            /// <summary>
            /// Represents a request to turn on the camera in a video call.
            /// </summary>
            VideoCallCameraOnRequest,

            /// <summary>
            /// Represents a response to turning on the camera in a video call.
            /// </summary>
            VideoCallCameraOnResponse,

            /// <summary>
            /// Represents a request to turn off the camera in a video call.
            /// </summary>
            VideoCallCameraOffRequest,

            /// <summary>
            /// Represents a response to turning off the camera in a video call.
            /// </summary>
            VideoCallCameraOffResponse,

            /// <summary>
            /// Represents a request to end a video call for the sender.
            /// </summary>
            EndVideoCallRequest,

            /// <summary>
            /// Represents a response to ending a video call for the sender.
            /// </summary>
            EndVideoCallResponse_Sender,

            /// <summary>
            /// Represents a response to ending a video call for the receiver.
            /// </summary>
            EndVideoCallResponse_Reciever,

            /// <summary>
            /// Represents a request for message history.
            /// </summary>
            MessageHistoryRequest,

            /// <summary>
            /// Represents a response containing message history.
            /// </summary>
            MessageHistoryResponse,

            /// <summary>
            /// Represents an update for online status.
            /// </summary>
            OnlineUpdate,

            /// <summary>
            /// Represents an update for offline status.
            /// </summary>
            OfflineUpdate,

            /// <summary>
            /// Represents a request for an audio call.
            /// </summary>
            AudioCallRequest,

            /// <summary>
            /// Represents a successful response to an audio call request for the sender.
            /// </summary>
            SuccessfulAudioCallResponse_Sender,

            /// <summary>
            /// Represents a successful response to an audio call request for the receiver.
            /// </summary>
            SuccessfulAudioCallResponse_Reciever,

            /// <summary>
            /// Represents a failed response to an audio call request.
            /// </summary>
            FailedAudioCallResponse,

            /// <summary>
            /// Represents a request for accepting an audio call.
            /// </summary>
            AudioCallAcceptanceRequest,

            /// <summary>
            /// Represents a request for denying an audio call.
            /// </summary>
            AudioCallDenialRequest,

            /// <summary>
            /// Represents a response to accepting an audio call.
            /// </summary>
            AudioCallAcceptanceResponse,

            /// <summary>
            /// Represents a response to denying an audio call.
            /// </summary>
            AudioCallDenialResponse,

            /// <summary>
            /// Represents a request to end an audio call for the sender.
            /// </summary>
            EndAudioCallRequest,

            /// <summary>
            /// Represents a response to ending an audio call for the sender.
            /// </summary>
            EndAudioCallResponse_Sender,

            /// <summary>
            /// Represents a response to ending an audio call for the receiver.
            /// </summary>
            EndAudioCallResponse_Reciever,

            /// <summary>
            /// Represents a request to delete a message.
            /// </summary>
            DeleteMessageRequest,

            /// <summary>
            /// Represents a response to deleting a message.
            /// </summary>
            DeleteMessageResponse,

            /// <summary>
            /// Represents a request to update the profile picture.
            /// </summary>
            UpdateProfilePictureRequest,

            /// <summary>
            /// Represents a response to updating the profile picture for the sender.
            /// </summary>
            UpdateProfilePictureResponse_Sender,

            /// <summary>
            /// Represents a response to updating the profile picture for the chat user receiver.
            /// </summary>
            UpdateProfilePictureResponse_ChatUserReciever,

            /// <summary>
            /// Represents a response to updating the profile picture for the contact receiver.
            /// </summary>
            UpdateProfilePictureResponse_ContactReciever,

            /// <summary>
            /// Represents a request to update the profile status.
            /// </summary>
            UpdateProfileStatusRequest,

            /// <summary>
            /// Represents a response to updating the profile status for the sender.
            /// </summary>
            UpdateProfileStatusResponse_Sender,

            /// <summary>
            /// Represents a response to updating the profile status for the receiver.
            /// </summary>
            UpdateProfileStatusResponse_Reciever,
        }

        #endregion
    }
}
