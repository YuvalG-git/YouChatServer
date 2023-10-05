CREATE TABLE [dbo].[UserDetails] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Username]     NVARCHAR (30) NOT NULL,
    [Password]     NVARCHAR (30) NOT NULL,
    [FirstName]    NVARCHAR (30) NOT NULL,
    [LastName]     NVARCHAR (30) NOT NULL,
    [EmailAddress] NVARCHAR (30) NOT NULL,
    [City]         NVARCHAR (40) NOT NULL,
    [BirthDate] DATE NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

