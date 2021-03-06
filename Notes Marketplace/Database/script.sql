USE [master]
GO
/****** Object:  Database [NoteMarketPlaceDatabase]    Script Date: 08-03-2021 11.18.59 AM ******/
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
/****** Object:  Table [dbo].[DownloadedNotes]    Script Date: 08-03-2021 11.19.00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DownloadedNotes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NoteID] [int] NOT NULL,
	[SellerID] [int] NOT NULL,
	[BuyersID] [int] NOT NULL,
	[IsSellerHasAllowedDownload] [bit] NOT NULL,
	[AttachmentPath] [varchar](max) NULL,
	[IsAttachmentDownloaded] [bit] NOT NULL,
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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ManageSystemConfiguration]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ManageSystemConfiguration](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SupportEmail] [varchar](100) NOT NULL,
	[OtherEmail] [varchar](50) NULL,
	[SupportPhoneNo] [varchar](50) NOT NULL,
	[FacebookURL] [varchar](100) NOT NULL,
	[TwitterURL] [varchar](100) NOT NULL,
	[LinkedinURL] [varchar](100) NOT NULL,
	[DefaultBookImage] [varchar](max) NOT NULL,
	[DefaultUserImage] [varchar](max) NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ManageSystemConfiguration] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Note]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Note](
	[NoteID] [int] IDENTITY(1,1) NOT NULL,
	[SellerID] [int] NOT NULL,
	[ActionedBy] [int] NULL,
	[NoteTitle] [varchar](100) NOT NULL,
	[Category] [int] NOT NULL,
	[DisplayPicture] [varchar](max) NULL,
	[AdminRemarks] [varchar](max) NULL,
	[NoteAttachment] [varchar](max) NOT NULL,
	[NoteSize] [decimal](18, 0) NULL,
	[NoteType] [int] NOT NULL,
	[Pages] [int] NULL,
	[NoteDescription] [varchar](max) NOT NULL,
	[Country] [int] NOT NULL,
	[Institute] [varchar](200) NULL,
	[CourseName] [varchar](100) NULL,
	[CourseCode] [varchar](100) NULL,
	[Professor] [varchar](100) NULL,
	[SellFor] [int] NOT NULL,
	[NotePreview] [varchar](max) NULL,
	[SellPrice] [decimal](18, 0) NULL,
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
/****** Object:  Table [dbo].[NoteCategory]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoteCategory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [varchar](100) NOT NULL,
	[CategoryDescription] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_NoteCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoteCountry]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoteCountry](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CountryName] [varchar](50) NOT NULL,
	[CountryCode] [varchar](10) NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_NoteCountry] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoteReview]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoteReview](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NoteID] [int] NOT NULL,
	[AgainstDownloadsID] [int] NOT NULL,
	[Ratings] [decimal](18, 0) NOT NULL,
	[Feedback] [varchar](max) NOT NULL,
	[Inappropriate] [bit] NULL,
	[BuyersID] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DownloadedNotes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoteType]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoteType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [varchar](100) NOT NULL,
	[TypeDescription] [varchar](max) NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_NoteType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Profile]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Profile](
	[ID] [int] IDENTITY(1,1) NOT NULL,
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
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReferenceData]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferenceData](
	[ID] [int] NOT NULL,
	[Value] [varchar](100) NOT NULL,
	[DataValue] [varchar](100) NOT NULL,
	[RefCategory] [varchar](100) NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ReferenceData] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RejectedNotes]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RejectedNotes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NoteID] [int] NOT NULL,
	[BuyersID] [int] NOT NULL,
	[AgainstDownloadsID] [int] NOT NULL,
	[RejectedBy] [int] NOT NULL,
	[Remarks] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_RejectedNotes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[ID] [int] IDENTITY(1,1) NOT NULL,
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
/****** Object:  Table [dbo].[Users]    Script Date: 08-03-2021 11.19.01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
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
SET IDENTITY_INSERT [dbo].[DownloadedNotes] ON 

INSERT [dbo].[DownloadedNotes] ([ID], [NoteID], [SellerID], [BuyersID], [IsSellerHasAllowedDownload], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadedDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (3, 1005, 12, 12, 1, N'~/PdfNotes/sample215941651.pdf', 1, NULL, 0, CAST(0 AS Decimal(18, 0)), N'Operating System', N'CA', NULL, NULL, NULL, NULL)
INSERT [dbo].[DownloadedNotes] ([ID], [NoteID], [SellerID], [BuyersID], [IsSellerHasAllowedDownload], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadedDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (1002, 4, 11, 10, 1, N'~/PdfNotes/sample214853998.pdf', 0, NULL, 1, CAST(215 AS Decimal(18, 0)), N'Computer Science', N'IT', NULL, NULL, NULL, NULL)
INSERT [dbo].[DownloadedNotes] ([ID], [NoteID], [SellerID], [BuyersID], [IsSellerHasAllowedDownload], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadedDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (1004, 1006, 12, 7, 1, N'~/PdfNotes/sample211725244.pdf', 1, NULL, 0, CAST(0 AS Decimal(18, 0)), N'Computer Network', N'IT', NULL, NULL, NULL, NULL)
INSERT [dbo].[DownloadedNotes] ([ID], [NoteID], [SellerID], [BuyersID], [IsSellerHasAllowedDownload], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadedDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (1005, 5, 11, 7, 0, NULL, 0, NULL, 1, CAST(215 AS Decimal(18, 0)), N'Basic Computer engineering Tech India Publication Series', N'IT', NULL, NULL, NULL, NULL)
INSERT [dbo].[DownloadedNotes] ([ID], [NoteID], [SellerID], [BuyersID], [IsSellerHasAllowedDownload], [AttachmentPath], [IsAttachmentDownloaded], [AttachmentDownloadedDate], [IsPaid], [PurchasedPrice], [NoteTitle], [NoteCategory], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (1006, 4, 11, 10, 0, NULL, 0, NULL, 1, CAST(215 AS Decimal(18, 0)), N'Computer Science', N'IT', NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[DownloadedNotes] OFF
GO
SET IDENTITY_INSERT [dbo].[Note] ON 

INSERT [dbo].[Note] ([NoteID], [SellerID], [ActionedBy], [NoteTitle], [Category], [DisplayPicture], [AdminRemarks], [NoteAttachment], [NoteSize], [NoteType], [Pages], [NoteDescription], [Country], [Institute], [CourseName], [CourseCode], [Professor], [SellFor], [NotePreview], [SellPrice], [Status], [ApprovedBy], [ApprovedDate], [PublishedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (4, 11, NULL, N'Computer Science', 1, N'~/Image/search2214853943.png', NULL, N'~/PdfNotes/sample214853998.pdf', CAST(15 AS Decimal(18, 0)), 1, 204, N'This book is about basics of computer science.', 1, N'University of California', N'Computer Engineering', N'248705', N'Mr. Richard Brown', 4, N'~/PreviewOfNotes/sample214854026.pdf', CAST(215 AS Decimal(18, 0)), 6, NULL, NULL, CAST(N'2021-02-24T22:48:54.170' AS DateTime), NULL, NULL, 1)
INSERT [dbo].[Note] ([NoteID], [SellerID], [ActionedBy], [NoteTitle], [Category], [DisplayPicture], [AdminRemarks], [NoteAttachment], [NoteSize], [NoteType], [Pages], [NoteDescription], [Country], [Institute], [CourseName], [CourseCode], [Professor], [SellFor], [NotePreview], [SellPrice], [Status], [ApprovedBy], [ApprovedDate], [PublishedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (5, 11, NULL, N'Basic Computer engineering Tech India Publication Series', 1, N'~/Image/search3210517659.png', NULL, N'~/PdfNotes/sample210517726.pdf', CAST(15 AS Decimal(18, 0)), 1, 205, N'This book is about Computer Engineering.', 1, N'University of California', N'Computer Engineering', N'248706', N'Mr. Richard Brown', 4, N'~/PreviewOfNotes/sample210517755.pdf', CAST(215 AS Decimal(18, 0)), 6, NULL, NULL, CAST(N'2021-02-26T10:05:17.773' AS DateTime), NULL, NULL, 1)
INSERT [dbo].[Note] ([NoteID], [SellerID], [ActionedBy], [NoteTitle], [Category], [DisplayPicture], [AdminRemarks], [NoteAttachment], [NoteSize], [NoteType], [Pages], [NoteDescription], [Country], [Institute], [CourseName], [CourseCode], [Professor], [SellFor], [NotePreview], [SellPrice], [Status], [ApprovedBy], [ApprovedDate], [PublishedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (1005, 12, NULL, N'Operating System', 2, N'~/Image/search1215941230.png', NULL, N'~/PdfNotes/sample215941651.pdf', CAST(15 AS Decimal(18, 0)), 1, 150, N'This book is about Operating System. ', 2, N'University of California', N'Computer Engineering', N'248705', N'Mr. Richard Brown', 5, N'~/PreviewOfNotes/sample215941741.pdf', CAST(0 AS Decimal(18, 0)), 6, NULL, NULL, CAST(N'2021-03-01T21:59:42.277' AS DateTime), NULL, NULL, 1)
INSERT [dbo].[Note] ([NoteID], [SellerID], [ActionedBy], [NoteTitle], [Category], [DisplayPicture], [AdminRemarks], [NoteAttachment], [NoteSize], [NoteType], [Pages], [NoteDescription], [Country], [Institute], [CourseName], [CourseCode], [Professor], [SellFor], [NotePreview], [SellPrice], [Status], [ApprovedBy], [ApprovedDate], [PublishedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (1006, 12, NULL, N'Computer Network', 1, N'~/Image/search6211725209.png', NULL, N'~/PdfNotes/sample211725244.pdf', CAST(15 AS Decimal(18, 0)), 1, 300, N'This book is about Computer Network.', 2, N'New York University', N'Information Technology', N'248707', N'Mr.David ', 5, N'~/PreviewOfNotes/sample211725255.pdf', CAST(0 AS Decimal(18, 0)), 9, NULL, NULL, CAST(N'2021-03-01T22:17:25.257' AS DateTime), NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[Note] OFF
GO
SET IDENTITY_INSERT [dbo].[NoteCategory] ON 

INSERT [dbo].[NoteCategory] ([ID], [CategoryName], [CategoryDescription], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1, N'IT', N'IT', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[NoteCategory] ([ID], [CategoryName], [CategoryDescription], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (2, N'CA', N'CA', NULL, NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[NoteCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[NoteCountry] ON 

INSERT [dbo].[NoteCountry] ([ID], [CountryName], [CountryCode], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (1, N'India', N'+91', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[NoteCountry] ([ID], [CountryName], [CountryCode], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (2, N'USA', N'+1', NULL, NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[NoteCountry] OFF
GO
SET IDENTITY_INSERT [dbo].[NoteType] ON 

INSERT [dbo].[NoteType] ([ID], [TypeName], [TypeDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (1, N'Handwritten', N'Book written By hands.', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[NoteType] ([ID], [TypeName], [TypeDescription], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (2, N'Story Book', N'All type of story book', NULL, NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[NoteType] OFF
GO
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (1, N'Male', N'M', N'Gender', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (2, N'Female', N'Fe', N'Gender', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (3, N'Unknown', N'U', N'Gender', NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (4, N'Paid', N'P', N'Selling Mode', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (5, N'Free', N'F', N'Selling Mode', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (6, N'Draft', N'Draft', N'Notes Status', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (7, N'Submitted For Review', N'Submitted For Review', N'Note Status', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (8, N'In Review', N'In Review', N'Note Status', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (9, N'Published', N'Approved', N'Note Status', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (10, N'Rejected', N'Rejected', N'Note Status', NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[ReferenceData] ([ID], [Value], [DataValue], [RefCategory], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [IsActive]) VALUES (11, N'Removed', N'Removed', N'Note Status', NULL, NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[UserRole] ON 

INSERT [dbo].[UserRole] ([ID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (1, N'user', N'user', CAST(N'2020-03-12T00:00:00.000' AS DateTime), 1, CAST(N'2020-03-12T00:00:00.000' AS DateTime), 1, 1)
INSERT [dbo].[UserRole] ([ID], [Name], [Description], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (2, N'admin', N'admin', CAST(N'2020-03-12T00:00:00.000' AS DateTime), 1, CAST(N'2020-03-12T00:00:00.000' AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[UserRole] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([ID], [RoleID], [EmailID], [FirstName], [LastName], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (5, 1, N'3999vachauhan@gmail.com', N'Venisha', N'Chauhan', N'V123@nisha', 0, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Users] ([ID], [RoleID], [EmailID], [FirstName], [LastName], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (6, 1, N'jihanpatel40@gmail.com', N'Jihan', N'Patel', N'rV!!B9f%', 0, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Users] ([ID], [RoleID], [EmailID], [FirstName], [LastName], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (7, 1, N'jinalpatel11121999@gmail.com', N'Jinal', N'Patel', N'Jinal@1234', 0, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Users] ([ID], [RoleID], [EmailID], [FirstName], [LastName], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (8, 1, N'jinaltamboli2608@gmail.com', N'Jinal', N'Tamboli', N'Jinal#2608', 0, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Users] ([ID], [RoleID], [EmailID], [FirstName], [LastName], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (9, 1, N'kaushika1631981@gmail.com', N'Kaushika', N'Patel', N'Kaushika@123', 0, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Users] ([ID], [RoleID], [EmailID], [FirstName], [LastName], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (10, 1, N'nidakadri27@gmail.com', N'Nida', N'Kadri', N'Nida@27#', 0, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Users] ([ID], [RoleID], [EmailID], [FirstName], [LastName], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (11, 1, N'pateldhara1019@gmail.com', N'Dhara', N'Patel', N'Dhara@123#', 1, NULL, NULL, NULL, NULL, 1)
INSERT [dbo].[Users] ([ID], [RoleID], [EmailID], [FirstName], [LastName], [Password], [IsEmailVerified], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy], [IsActive]) VALUES (12, 1, N'machauhan1733@gmail.com', N'Maitri', N'Chauhan', N'Ma@123#tri', 0, NULL, NULL, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_User_EmailID]    Script Date: 08-03-2021 11.19.01 AM ******/
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
ALTER TABLE [dbo].[DownloadedNotes]  WITH CHECK ADD  CONSTRAINT [FK_DownloadedNotes_BuyserID] FOREIGN KEY([BuyersID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[DownloadedNotes] CHECK CONSTRAINT [FK_DownloadedNotes_BuyserID]
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
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_ActionBy] FOREIGN KEY([ActionedBy])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_ActionBy]
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
ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_SellerID] FOREIGN KEY([NoteID])
REFERENCES [dbo].[Note] ([NoteID])
GO
ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_SellerID]
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
ALTER TABLE [dbo].[Profile]  WITH CHECK ADD  CONSTRAINT [FK_Profile_Gender] FOREIGN KEY([Gender])
REFERENCES [dbo].[ReferenceData] ([ID])
GO
ALTER TABLE [dbo].[Profile] CHECK CONSTRAINT [FK_Profile_Gender]
GO
ALTER TABLE [dbo].[Profile]  WITH CHECK ADD  CONSTRAINT [FK_Profile_SellerID] FOREIGN KEY([SellerID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Profile] CHECK CONSTRAINT [FK_Profile_SellerID]
GO
ALTER TABLE [dbo].[RejectedNotes]  WITH CHECK ADD  CONSTRAINT [FK_RejectedNotes_BuyersID] FOREIGN KEY([BuyersID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[RejectedNotes] CHECK CONSTRAINT [FK_RejectedNotes_BuyersID]
GO
ALTER TABLE [dbo].[RejectedNotes]  WITH CHECK ADD  CONSTRAINT [FK_RejectedNotes_NoteID] FOREIGN KEY([NoteID])
REFERENCES [dbo].[Note] ([NoteID])
GO
ALTER TABLE [dbo].[RejectedNotes] CHECK CONSTRAINT [FK_RejectedNotes_NoteID]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserRole] FOREIGN KEY([RoleID])
REFERENCES [dbo].[UserRole] ([ID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UserRole]
GO
USE [master]
GO
ALTER DATABASE [NoteMarketPlaceDatabase] SET  READ_WRITE 
GO
