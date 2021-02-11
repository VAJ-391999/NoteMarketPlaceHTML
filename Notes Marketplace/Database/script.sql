USE [master]
GO
/****** Object:  Database [NoteMarketPlaceDatabase]    Script Date: 11-02-2021 7.23.53 PM ******/
CREATE DATABASE [NoteMarketPlaceDatabase]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'NoteMarketPlaceDatabase', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\NoteMarketPlaceDatabase.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'NoteMarketPlaceDatabase_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\NoteMarketPlaceDatabase_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [NoteMarketPlaceDatabase].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET ARITHABORT OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET  DISABLE_BROKER 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET RECOVERY FULL 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET  MULTI_USER 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET DB_CHAINING OFF 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'NoteMarketPlaceDatabase', N'ON'
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET QUERY_STORE = OFF
GO
USE [NoteMarketPlaceDatabase]
GO
/****** Object:  Table [dbo].[DownloadedNotes]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DownloadedNotes](
	[ID] [int] NOT NULL,
	[NoteID] [int] NOT NULL,
	[SellerID] [int] NOT NULL,
	[BuyersID] [int] NOT NULL,
	[IsSellerHasAllowedDownload] [bit] NOT NULL,
	[AttachmentDownloadedDate] [datetime] NULL,
	[IsPaid] [bit] NOT NULL,
	[PurchasedPrice] [decimal](18, 0) NULL,
	[NoteTitle] [varchar](100) NOT NULL,
	[NoteCategory] [varchar](100) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
 CONSTRAINT [PK_DownloadedNotes_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ManageSystemConfiguration]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ManageSystemConfiguration](
	[ID] [int] NOT NULL,
	[SupportEmail] [varchar](100) NOT NULL,
	[OtherEmail] [varchar](50) NULL,
	[SupportPhoneNo] [varchar](50) NOT NULL,
	[FacebookURL] [varchar](100) NOT NULL,
	[TwitterURL] [varchar](100) NOT NULL,
	[LinkedinURL] [varchar](100) NOT NULL,
	[DefaultBookImage] [varchar](max) NOT NULL,
	[DefaultUserImage] [varchar](max) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ManageSystemConfiguration] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Note]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Note](
	[NoteID] [int] NOT NULL,
	[SellerID] [int] NOT NULL,
	[ActionedBy] [int] NULL,
	[NoteTitle] [varchar](100) NOT NULL,
	[Category] [int] NOT NULL,
	[DisplayPicture] [varchar](max) NULL,
	[NoteAttachment] [varchar](max) NOT NULL,
	[NoteSize] [int] NOT NULL,
	[NoteType] [int] NOT NULL,
	[Pages] [int] NULL,
	[NoteDescription] [varchar](max) NOT NULL,
	[Country] [int] NOT NULL,
	[Institute] [varchar](200) NULL,
	[CourseName] [varchar](100) NULL,
	[CourseCode] [varchar](100) NULL,
	[Professor] [varchar](100) NULL,
	[SellFor] [int] NOT NULL,
	[NotePreview] [varchar](max) NOT NULL,
	[SellPrice] [decimal](18, 0) NOT NULL,
	[Status] [int] NOT NULL,
	[ApprovedBy] [int] NULL,
	[ApprovedDate] [datetime] NULL,
	[PublishedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED 
(
	[NoteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoteCategory]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoteCategory](
	[ID] [int] NOT NULL,
	[CategoryName] [varchar](100) NOT NULL,
	[CategoryDescription] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_NoteCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoteCountry]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoteCountry](
	[ID] [int] NOT NULL,
	[CountryName] [varchar](50) NOT NULL,
	[CountryCode] [varchar](10) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_NoteCountry] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoteReview]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoteReview](
	[ID] [int] NOT NULL,
	[SellerID] [int] NOT NULL,
	[NoteID] [int] NOT NULL,
	[AgainstDownloadsID] [int] NOT NULL,
	[Ratings] [decimal](18, 0) NOT NULL,
	[Feedback] [varchar](max) NOT NULL,
	[Inappropriate] [bit] NULL,
	[BuyersID] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DownloadedNotes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoteType]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoteType](
	[ID] [int] NOT NULL,
	[TypeName] [varchar](100) NOT NULL,
	[TypeDescription] [varchar](max) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_NoteType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Profile]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Profile](
	[ID] [int] NOT NULL,
	[SellerID] [int] NULL,
	[SecondaryEmailID] [varchar](50) NULL,
	[DOB] [date] NULL,
	[Gender] [int] NULL,
	[PhoneNo] [varchar](20) NOT NULL,
	[CountryCode] [int] NOT NULL,
	[ProfilePhoto] [varchar](max) NULL,
	[Address1] [varchar](100) NOT NULL,
	[Address2] [varchar](100) NOT NULL,
	[City] [varchar](50) NOT NULL,
	[State] [varchar](50) NOT NULL,
	[ZipCode] [varchar](50) NOT NULL,
	[Country] [varchar](50) NOT NULL,
	[University] [varchar](100) NULL,
	[College] [varchar](100) NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReferenceData]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferenceData](
	[ID] [int] NOT NULL,
	[Value] [varchar](100) NOT NULL,
	[DataValue] [varchar](100) NOT NULL,
	[RefCategory] [varchar](100) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ReferenceData] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RejectedNotes]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RejectedNotes](
	[ID] [int] NOT NULL,
	[SellerID] [int] NOT NULL,
	[NoteID] [int] NOT NULL,
	[BuyersID] [int] NOT NULL,
	[AgainstDownloadsID] [int] NOT NULL,
	[RejectedBy] [int] NOT NULL,
	[Remarks] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_RejectedNotes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[ID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11-02-2021 7.24.02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] NOT NULL,
	[RoleID] [int] NOT NULL,
	[EmailID] [varchar](50) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[Password] [varchar](24) NOT NULL,
	[IsEmailVerified] [bit] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_User_EmailID]    Script Date: 11-02-2021 7.24.06 PM ******/
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [UQ_User_EmailID] UNIQUE NONCLUSTERED 
(
	[EmailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ManageSystemConfiguration] ADD  CONSTRAINT [DF_ManageSystemConfiguration_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Note] ADD  CONSTRAINT [DF_Note_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[NoteCategory] ADD  CONSTRAINT [DF_NoteCategory_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[NoteCountry] ADD  CONSTRAINT [DF_NoteCountry_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[NoteReview] ADD  CONSTRAINT [DF_NoteReview_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[NoteType] ADD  CONSTRAINT [DF_NoteType_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Profile] ADD  CONSTRAINT [DF_Profile_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ReferenceData] ADD  CONSTRAINT [DF_ReferenceData_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[RejectedNotes] ADD  CONSTRAINT [DF_RejectedNotes_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_User_IsEmailVerified]  DEFAULT ((0)) FOR [IsEmailVerified]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[DownloadedNotes]  WITH CHECK ADD  CONSTRAINT [FK_DownloadedNotes_BuyersID] FOREIGN KEY([BuyersID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[DownloadedNotes] CHECK CONSTRAINT [FK_DownloadedNotes_BuyersID]
GO
ALTER TABLE [dbo].[DownloadedNotes]  WITH CHECK ADD  CONSTRAINT [FK_DownloadedNotes_NoteID] FOREIGN KEY([NoteID])
REFERENCES [dbo].[Note] ([NoteID])
GO
ALTER TABLE [dbo].[DownloadedNotes] CHECK CONSTRAINT [FK_DownloadedNotes_NoteID]
GO
ALTER TABLE [dbo].[DownloadedNotes]  WITH CHECK ADD  CONSTRAINT [FK_DownloadedNotes_SellerID] FOREIGN KEY([SellerID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[DownloadedNotes] CHECK CONSTRAINT [FK_DownloadedNotes_SellerID]
GO
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_ActionedBy] FOREIGN KEY([ActionedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_ActionedBy]
GO
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Category] FOREIGN KEY([Category])
REFERENCES [dbo].[NoteCategory] ([ID])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_Category]
GO
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Country] FOREIGN KEY([Country])
REFERENCES [dbo].[NoteCountry] ([ID])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_Country]
GO
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_NoteType] FOREIGN KEY([NoteType])
REFERENCES [dbo].[NoteType] ([ID])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_NoteType]
GO
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_SellerID] FOREIGN KEY([SellerID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_SellerID]
GO
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_SellFor] FOREIGN KEY([SellFor])
REFERENCES [dbo].[ReferenceData] ([ID])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_SellFor]
GO
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Status] FOREIGN KEY([Status])
REFERENCES [dbo].[ReferenceData] ([ID])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_Status]
GO
ALTER TABLE [dbo].[NoteReview]  WITH CHECK ADD  CONSTRAINT [FK_NoteReview_AgainstDownloadsID] FOREIGN KEY([AgainstDownloadsID])
REFERENCES [dbo].[DownloadedNotes] ([ID])
GO
ALTER TABLE [dbo].[NoteReview] CHECK CONSTRAINT [FK_NoteReview_AgainstDownloadsID]
GO
ALTER TABLE [dbo].[NoteReview]  WITH CHECK ADD  CONSTRAINT [FK_NoteReview_BuyersID] FOREIGN KEY([BuyersID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[NoteReview] CHECK CONSTRAINT [FK_NoteReview_BuyersID]
GO
ALTER TABLE [dbo].[NoteReview]  WITH CHECK ADD  CONSTRAINT [FK_NoteReview_NoteID] FOREIGN KEY([NoteID])
REFERENCES [dbo].[Note] ([NoteID])
GO
ALTER TABLE [dbo].[NoteReview] CHECK CONSTRAINT [FK_NoteReview_NoteID]
GO
ALTER TABLE [dbo].[NoteReview]  WITH CHECK ADD  CONSTRAINT [FK_NoteReview_SellerID] FOREIGN KEY([SellerID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[NoteReview] CHECK CONSTRAINT [FK_NoteReview_SellerID]
GO
ALTER TABLE [dbo].[Profile]  WITH CHECK ADD  CONSTRAINT [FK_Profile_Gender] FOREIGN KEY([Gender])
REFERENCES [dbo].[ReferenceData] ([ID])
GO
ALTER TABLE [dbo].[Profile] CHECK CONSTRAINT [FK_Profile_Gender]
GO
ALTER TABLE [dbo].[Profile]  WITH CHECK ADD  CONSTRAINT [FK_Profile_User] FOREIGN KEY([SellerID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Profile] CHECK CONSTRAINT [FK_Profile_User]
GO
ALTER TABLE [dbo].[RejectedNotes]  WITH CHECK ADD  CONSTRAINT [FK_RejectedNotes_AgainstDownloadsID] FOREIGN KEY([AgainstDownloadsID])
REFERENCES [dbo].[DownloadedNotes] ([ID])
GO
ALTER TABLE [dbo].[RejectedNotes] CHECK CONSTRAINT [FK_RejectedNotes_AgainstDownloadsID]
GO
ALTER TABLE [dbo].[RejectedNotes]  WITH CHECK ADD  CONSTRAINT [FK_RejectedNotes_NoteID] FOREIGN KEY([NoteID])
REFERENCES [dbo].[Note] ([NoteID])
GO
ALTER TABLE [dbo].[RejectedNotes] CHECK CONSTRAINT [FK_RejectedNotes_NoteID]
GO
ALTER TABLE [dbo].[RejectedNotes]  WITH CHECK ADD  CONSTRAINT [FK_RejectedNotes_RejecedBy] FOREIGN KEY([RejectedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[RejectedNotes] CHECK CONSTRAINT [FK_RejectedNotes_RejecedBy]
GO
ALTER TABLE [dbo].[RejectedNotes]  WITH CHECK ADD  CONSTRAINT [FK_RejectedNotes_SellerID] FOREIGN KEY([SellerID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[RejectedNotes] CHECK CONSTRAINT [FK_RejectedNotes_SellerID]
GO
ALTER TABLE [dbo].[RejectedNotes]  WITH CHECK ADD  CONSTRAINT [FK_RejectedNotes_Users] FOREIGN KEY([BuyersID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[RejectedNotes] CHECK CONSTRAINT [FK_RejectedNotes_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_User_UserRole] FOREIGN KEY([RoleID])
REFERENCES [dbo].[UserRole] ([ID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_User_UserRole]
GO
USE [master]
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET  READ_WRITE 
GO
