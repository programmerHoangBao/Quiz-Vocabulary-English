namespace back_end.Records
{
    public sealed record MessageRecord(string ResponseCode, string Message, int HttpStatus)
    {
        //The ResponseCode for a successful status starts with the character ‘S’ and has 4 characters, for example: S001.
        public static readonly MessageRecord UserRegistered = new("S001", "User registered successfully!", StatusCodes.Status201Created);
        public static readonly MessageRecord VerifySuccess = new("S002", "User verified successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord LoginSuccess = new("S003", "Login successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord FolderCreateSuccess = new("S004", "Created folder successfully!", StatusCodes.Status201Created);
        public static readonly MessageRecord FolderUpdateSuccess = new("S005", "Updated folder successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord FolderDeleteSuccess = new("S006", "Delete folder successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord GetFolderOfUserSuccess = new("S008", "Get folders of user successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord GetFolderByIdSuccess = new("S009", "Get folder successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord TopicCreateSuccess = new("S010", "Create topic successfully!", StatusCodes.Status201Created);
        public static readonly MessageRecord TopicUpdateSuccess = new("S011", "Update topic successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord TopicDeleteSuccess = new("S012", "Soft Delete topic successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord GetTopicByIdSuccess = new("S013", "Get topic by id successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord GetTopicsByFolderIdSuccess = new("S013", "Get topic by folderId successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord VocaboluryCreationSuccess = new("S014", "Vocabolury creates successfully!", StatusCodes.Status201Created);
        public static readonly MessageRecord GetVocaboluriesByTopicIdSuccess = new("S015", "Get vocaboluries by topic id successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord GetVocaboluryByIdSuccess = new("S016", "Get vocabolury by id successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord VocaboluryUpdateSuccess = new("S017", "Vocabolury updates successfully!", StatusCodes.Status200OK);
        public static readonly MessageRecord VocaboluryDeleteSuccess = new("S018", "Vocabolury deletes successfully!", StatusCodes.Status200OK);
    }
}
