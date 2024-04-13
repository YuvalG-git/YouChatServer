using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouChatServer
{
    public static class EnumHandler
    {
        public enum UserAuthentication_Enum
        {
            Login,
            Registration,
            PasswordUpdate,
            PasswordRestart
        }
        /// <summary>
        /// Enum used to ...
        /// </summary>
        public enum SmtpMessageType_Enum
        {
            RegistrationMessage,
            LoginMessage,
            PasswordRenewalMessage,
        }
        public enum CommunicationMessageID_Enum
        {
            loginRequest,
            loginRequest_SmtpLoginMessage,
            loginResponse_SmtpLoginMessage,
            loginResponse_FailedLogin,
            LoginRequest_SmtpLoginCode,
            LoginResponse_SuccessfulSmtpLoginCode,
            LoginResponse_FailedSmtpLoginCode,
            RegistrationRequest_SmtpRegistrationMessage,
            RegistrationResponse_SmtpRegistrationMessage,
            RegistrationRequest_SmtpRegistrationCode,
            RegistrationResponse_SmtpRegistrationCode,
            RegistrationRequest_Registration,
            RegistrationResponse_SuccessfulRegistration,
            RegistrationResponse_FailedRegistration,
            EncryptionClientPublicKeySender,
            EncryptionServerPublicKeyAndSymmetricKeyReciever,
            EncryptionServerPublicKeyReciever,
            EncryptionSymmetricKeyReciever,
            EncryptionRenewKeys,
            FriendRequestSender,
            UploadProfilePictureRequest,
            UploadProfilePictureResponse,
            UploadStatusRequest,
            UploadStatusResponse,
            CaptchaImageRequest,
            CaptchaImageResponse,
            CaptchaCodeRequest,
            SuccessfulCaptchaCodeResponse,
            FailedCaptchaCodeResponse,
            CaptchaImageAngleRequest,
            CaptchaImageAngleResponse,
            SuccessfulCaptchaImageAngleResponse,
            FailedCaptchaImageAngleResponse,
            PersonalVerificationAnswersRequest,
            SuccessfulPersonalVerificationAnswersResponse_UpdatePassword,
            SuccessfulPersonalVerificationAnswersResponse_SetUserProfilePicture,
            SuccessfulPersonalVerificationAnswersResponse_SetUserStatus,
            SuccessfulPersonalVerificationAnswersResponse_OpenChat,
            SuccessfulPersonalVerificationAnswersResponse_HandleError,
            FailedPersonalVerificationAnswersResponse,
            Disconnect,
            SendMessageRequest,
            SendMessageResponse,
            UdpAudioConnectionRequest,
            UdpAudioConnectionResponse,
            PasswordUpdateRequest,
            SuccessfulPasswordUpdateResponse,
            FailedPasswordUpdateResponse_UnmatchedDetails,
            FailedPasswordUpdateResponse_PasswordExist,
            ErrorHandlePasswordUpdateResponse,
            InitialProfileSettingsCheckRequest,
            InitialProfileSettingsCheckResponse_SetUserProfilePicture,
            InitialProfileSettingsCheckResponse_SetUserStatus,
            InitialProfileSettingsCheckResponse_OpenChat,
            InitialProfileSettingsCheckResponse_HandleError,
            UserDetailsRequest,
            UserDetailsResponse,
            ChatSettingsChangeRequest,
            ChatSettingsChangeResponse
        }
    }
}
