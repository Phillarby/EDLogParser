CREATE TABLE [EDDATA].[Profile]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Commander] NVARCHAR(150) NOT NULL, 
    [Email] NVARCHAR(150) NOT NULL, 
	[EmailValidated] BIT NOT NULL DEFAULT(0),
    [Password] NVARCHAR(256) NOT NULL, 
    [SecurityToken] NVARCHAR(50) NOT NULL, 
    [TokenExpiry] DATETIME NOT NULL, 
    CONSTRAINT FK_Profile_Asset FOREIGN KEY (Id) REFERENCES [EDDATA].[Asset] (Id)
)
