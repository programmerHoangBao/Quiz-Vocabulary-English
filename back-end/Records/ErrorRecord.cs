namespace back_end.Records
{
    public sealed record ErrorRecord(string ResponseCode, string Message, int HttpStatus)
    {
        // The ResponseCode for an error status starts with the character ‘E’ and has 4 characters, for example: E001.
        public static readonly ErrorRecord UserExist = new("E001", "User already exists!", StatusCodes.Status409Conflict);
        public static readonly ErrorRecord RegisterFailed = new("E002", "Register user failed!", StatusCodes.Status500InternalServerError);
        public static readonly ErrorRecord UserNotFound = new("E003", "User not found!", StatusCodes.Status404NotFound);
        public static readonly ErrorRecord UserIsNotVerify = new("E004", "User is not verified!", StatusCodes.Status403Forbidden);
        public static readonly ErrorRecord OtpExpiry = new("E005", "OTP has expired!", StatusCodes.Status403Forbidden);
        public static readonly ErrorRecord VerifyFailed = new("E006", "Verify user failed!", StatusCodes.Status500InternalServerError);
        public static readonly ErrorRecord RequestInvalid = new("E007", "Request is invalid!", StatusCodes.Status400BadRequest);
        public static readonly ErrorRecord RequestNotFound = new("E008", "Request not found!", StatusCodes.Status404NotFound);
        public static readonly ErrorRecord InternalServerError = new("E009", "Internal server error!", StatusCodes.Status500InternalServerError);
        public static readonly ErrorRecord LoginFailed = new("E010", "The email or password incorrect!", StatusCodes.Status401Unauthorized);
        public static readonly ErrorRecord Unauthorized = new("E011", "User does not have unauthorization!", StatusCodes.Status401Unauthorized);
        public static readonly ErrorRecord Forbidden = new("E012", "User does not have permission!", StatusCodes.Status403Forbidden);
        public static readonly ErrorRecord CreateFolderFailed = new("E012", "Created folder failed!", StatusCodes.Status400BadRequest);
        public static readonly ErrorRecord UpdateFolderFailed = new("E013", "Updated folder failed!", StatusCodes.Status400BadRequest);
        public static readonly ErrorRecord SoftDeleteFailed = new("E014", "Delete folder failed!", StatusCodes.Status400BadRequest);
        public static readonly ErrorRecord NoData = new("E015", "No data!", StatusCodes.Status404NotFound);
        public static readonly ErrorRecord FolderNotFound = new("E016", "Folder not found!", StatusCodes.Status404NotFound);
    }
}
