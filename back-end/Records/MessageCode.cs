namespace back_end.Records
{
    public sealed record MessageCode(string ResponseCode, string Message, int HttpStatus)
    {
        // The ResponseCode for an error status starts with the character ‘E’ and has 4 characters, for example: E001.
        public static readonly MessageCode UserExist = new("E001", "User already exists!", StatusCodes.Status409Conflict);
        public static readonly MessageCode RegisterFailed = new("E002", "Register user failed!", StatusCodes.Status500InternalServerError);
        public static readonly MessageCode UserNotFound = new("E003", "User not found!", StatusCodes.Status404NotFound);
        public static readonly MessageCode UserIsNotVerify = new("E004", "User is not verified!", StatusCodes.Status403Forbidden);
        public static readonly MessageCode OtpExpiry = new("E005", "OTP has expired!", StatusCodes.Status403Forbidden);
        public static readonly MessageCode VerifyFailed = new("E006", "Verify user failed!", StatusCodes.Status500InternalServerError);
        public static readonly MessageCode RequestInvalid = new("E007", "Request is invalid!", StatusCodes.Status400BadRequest);
        public static readonly MessageCode RequestNotFound = new("E008", "Request not found!", StatusCodes.Status404NotFound);
        public static readonly MessageCode InternalServerError = new("E009", "Internal server error!", StatusCodes.Status500InternalServerError);
        public static readonly MessageCode LoginFailed = new("E010", "The email or password incorrect!", StatusCodes.Status401Unauthorized);

        //The ResponseCode for a successful status starts with the character ‘S’ and has 4 characters, for example: S001.
        public static readonly MessageCode UserRegistered = new("S001", "User registered successfully!", StatusCodes.Status201Created);
        public static readonly MessageCode VerifySuccess = new("S002", "User verified successfully!", StatusCodes.Status200OK);
        public static readonly MessageCode LoginSuccess = new("S003", "Login successfully!", StatusCodes.Status200OK);
    }
}
