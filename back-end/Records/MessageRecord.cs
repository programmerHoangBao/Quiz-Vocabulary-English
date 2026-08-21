namespace back_end.Records
{
    public sealed record MessageRecord(string ResponseCode, string Message, int HttpStatus)
    {
        //The ResponseCode for a successful status starts with the character ‘S’ and has 4 characters, for example: S001.
        public static readonly MessageRecord UserRegistered = new("S001", "User registered successfully!", StatusCodes.Status201Created);
        public static readonly MessageRecord VerifySuccess = new("S002", "User verified successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord LoginSuccess = new("S003", "Login successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord CreateFolderSuccess = new("S004", "Created folder successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord UpdateFolderSuccess = new("S005", "Updated folder successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord SoftDeleteSuccess = new("S006", "Delete folder successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord GetFolderOfUserSuccess = new("S008", "Get folders of user successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord GetFolderByIdSuccess = new("S009", "Get folder successfully!", StatusCodes.Status200OK);
    }
}
