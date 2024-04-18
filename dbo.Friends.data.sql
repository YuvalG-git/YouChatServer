SET IDENTITY_INSERT [dbo].[Friends] ON
INSERT INTO [dbo].[Friends] ([Id], [UserName], [Friend-1], [Friend-2]) VALUES (26, N'yuval', N'tamar', N'sivan')
INSERT INTO [dbo].[Friends] ([Id], [UserName], [Friend-1], [Friend-2]) VALUES (27, N'tamar', N'yuval', NULL)
INSERT INTO [dbo].[Friends] ([Id], [UserName], [Friend-1], [Friend-2]) VALUES (28, N'sivan', N'yuval', NULL)
INSERT INTO [dbo].[Friends] ([Id], [UserName], [Friend-1], [Friend-2]) VALUES (29, N'einat', N'yuval', NULL)
SET IDENTITY_INSERT [dbo].[Friends] OFF
