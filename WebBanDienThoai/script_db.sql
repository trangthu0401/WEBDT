USE [DemoWebBanDienThoai]
GO
/* ============================================================
   1. XÓA BẢNG CŨ NẾU ĐÃ TỒN TẠI (THEO THỨ TỰ ĐỂ TRÁNH LỖI FK)
   ============================================================ */
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Shipping]') AND type in (N'U')) DROP TABLE [dbo].[Shipping]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type in (N'U')) DROP TABLE [dbo].[Payments]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReviewDetails]') AND type in (N'U')) DROP TABLE [dbo].[ReviewDetails]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reviews]') AND type in (N'U')) DROP TABLE [dbo].[Reviews]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND type in (N'U')) DROP TABLE [dbo].[OrderDetails]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U')) DROP TABLE [dbo].[Orders]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FavoriteDetails]') AND type in (N'U')) DROP TABLE [dbo].[FavoriteDetails]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Favorites]') AND type in (N'U')) DROP TABLE [dbo].[Favorites]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CartItems]') AND type in (N'U')) DROP TABLE [dbo].[CartItems]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductVariants]') AND type in (N'U')) DROP TABLE [dbo].[ProductVariants]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U')) DROP TABLE [dbo].[Products]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Brands]') AND type in (N'U')) DROP TABLE [dbo].[Brands]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ADDRESS]') AND type in (N'U')) DROP TABLE [dbo].[ADDRESS]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND type in (N'U')) DROP TABLE [dbo].[Notifications]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CUSTOMER]') AND type in (N'U')) DROP TABLE [dbo].[CUSTOMER]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ACCOUNT]') AND type in (N'U')) DROP TABLE [dbo].[ACCOUNT]
GO


/****** Object:  Table [dbo].[ACCOUNT]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ACCOUNT](
	[AccountID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Password] [varchar](255) NOT NULL,
	[Role] [nvarchar](20) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[IsActive] [bit] NULL,
	[Phone] [nvarchar](15) NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ADDRESS]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ADDRESS](
	[AddressID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[Street] [nvarchar](255) NOT NULL,
	[District] [nvarchar](100) NULL,
	[City] [nvarchar](100) NULL,
	[Country] [nvarchar](100) NULL,
	[PostalCode] [nvarchar](20) NULL,
	[IsDefault] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[AddressID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Brands]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brands](
	[BrandId] [int] IDENTITY(1,1) NOT NULL,
	[BrandName] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CartItems]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartItems](
	[CartItemId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[VariantId] [int] NULL,
	[Quantity] [int] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CartItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CUSTOMER]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CUSTOMER](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](15) NULL,
	[Gender] [nvarchar](10) NULL,
	[BirthDate] [date] NULL,
	[CustomerType] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FavoriteDetails]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FavoriteDetails](
	[FavoriteDetailId] [int] IDENTITY(1,1) NOT NULL,
	[FavoriteId] [int] NULL,
	[VariantId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[FavoriteDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Favorites]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Favorites](
	[FavoriteId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[FavoriteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notifications]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifications](
	[NotificationId] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NULL,
	[Message] [nvarchar](255) NULL,
	[Type] [nvarchar](50) NULL,
	[IsRead] [bit] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[VariantId] [int] NULL,
	[Quantity] [int] NULL,
	[UnitPrice] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[OrderDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[ShippingFullName] [nvarchar](100) NULL,
	[ShippingPhone] [nvarchar](15) NULL,
	[ShippingStreet] [nvarchar](255) NULL,
	[ShippingDistrict] [nvarchar](100) NULL,
	[ShippingCity] [nvarchar](100) NULL,
	[ShippingCountry] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[PaymentId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[PaymentMethod] [nvarchar](50) NULL,
	[PaymentDate] [datetime] NULL,
	[PaymentStatus] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[BrandId] [int] NOT NULL,
	[Chipset] [nvarchar](50) NULL,
	[OperatingSystem] [nvarchar](30) NULL,
	[BatteryCapacity] [smallint] NULL,
	[ChargerIncluded] [bit] NULL,
	[ScreenSize] [decimal](4, 2) NULL,
	[ScreenTech] [nvarchar](40) NULL,
	[RefreshRate] [smallint] NULL,
	[RearCamera] [nvarchar](100) NULL,
	[FrontCamera] [nvarchar](50) NULL,
	[Weight] [decimal](5, 2) NULL,
	[Dimensions] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NULL,
	[ReleaseDate] [date] NULL,
	[MainImage] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductVariants]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductVariants](
	[VariantId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[Color] [nvarchar](30) NULL,
	[Storage] [nvarchar](20) NULL,
	[RAM] [nvarchar](20) NULL,
	[Price] [decimal](18, 2) NULL,
	[DiscountPrice] [decimal](18, 2) NULL,
	[Stock] [int] NULL,
	[ImageUrl] [nvarchar](255) NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[VariantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewDetails]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewDetails](
	[ReviewDetailId] [int] IDENTITY(1,1) NOT NULL,
	[ReviewId] [int] NULL,
	[VariantId] [int] NULL,
	[Rating] [int] NULL,
	[Comment] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ReviewDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reviews]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reviews](
	[ReviewId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Shipping]    Script Date: 3/8/2026 3:57:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shipping](
	[ShippingId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NULL,
	[Carrier] [nvarchar](100) NULL,
	[TrackingNumber] [nvarchar](100) NULL,
	[ShippedDate] [datetime] NULL,
	[EstimatedDelivery] [datetime] NULL,
	[DeliveredDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[Note] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ShippingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ACCOUNT] ON 

INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (1, N'admin@shop.com', N'$2a$11$zcE.ameAB22zQlJB5Ra/n.8Gpxsp2VBq0yDoJOpQWV6tOxX2LI1rm', N'Admin', CAST(N'2025-11-20T20:04:19.373' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (2, N'nguyenvana@gmail.com', N'$2a$11$7Lu7cgf2c/EMRAj.Z62Kre6OvZog.XXXNsNmHApEcFFX3Op.Yb8gy', N'Customer', CAST(N'2025-11-20T20:04:19.373' AS DateTime), 0, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (3, N'tranthib@gmail.com', N'$2a$11$E9lJULzEcRki8WO2NsO9A.wPwc9Xv6K/YZuw2HtKtJQycOoJzTvnG', N'Customer', CAST(N'2025-11-20T20:04:19.373' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (4, N'phamminhc@gmail.com', N'user789', N'Customer', CAST(N'2025-11-20T20:04:19.373' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (5, N'lethidungd@gmail.com', N'user012', N'Customer', CAST(N'2025-11-20T20:04:19.373' AS DateTime), 0, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (6, N'ngovantranh@gmail.com', N'user345', N'Customer', CAST(N'2025-11-20T20:04:19.373' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (7, N'Nguyen@gmail.com', N'$2a$11$MMVWEXEyD0IDjtvuhlA07uqDI.0wIiD05Rlxo3Q5asFamAlMznubK', N'Customer', CAST(N'2025-11-20T22:57:27.737' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (8, N'VanToan@gmail.com', N'$2a$11$zhCUufl.kE.rS7X21Ov2ReUHA3DH71zV99simecMfQ9XZiA.Njeiy', N'Customer', CAST(N'2025-11-20T23:02:02.863' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (9, N'trung@gmail.com', N'$2a$11$bcjKci3tVrWk9EQuhrDni.36.Mea9HdbUcI3Mfxb4CuaM1TyFpCkW', N'Customer', CAST(N'2025-11-21T09:34:44.093' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (11, N'trung123@gmail.com', N'$2a$11$5loWJOR/DGyHgPQq5SJ5wetGlZ0ihGjuhujX6Y.jjPXBp7JXyhAnO', N'Customer', CAST(N'2025-11-21T09:35:38.787' AS DateTime), 0, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (12, N'trang123@gmail.com', N'$2a$11$x6p.VC1/KxxDNdzMnkq7XO3LNgpP0q9rQsLvzcmq47l3pJcQLzw6.', N'Customer', CAST(N'2025-11-23T23:17:55.127' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (13, N'NguyenThiBinh2@gmail.com', N'$2a$11$Rxe9.4S63Za4v6VD4L0Os.HcAIkUE0b46qBYLElNl4ZbTCYYsAeGe', N'Customer', CAST(N'2025-11-28T10:35:53.463' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (14, N'NguyenThiBinh3@gmail.com', N'$2a$11$tsLJrr7HF02B3un8yQNGauN01/pHI0LG.fSODnxdSAfpELy3E6pvq', N'Customer', CAST(N'2025-11-28T10:43:24.347' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (15, N'vanA@gmail.com', N'$2a$11$XHYTFbx8behjOWTO4ZaEZutIYgjfXaynU5B1NSIkA0DF9F6e5f/06', N'Customer', CAST(N'2025-11-28T12:31:10.230' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (16, N'ngtttrang040105@gmail.com', N'$2a$11$3Im3.4RfpsAu5U.6U0sI.OaGtgJXXkCdde4pTWEZ/RVVjYbWtbuqW', N'Customer', CAST(N'2025-11-28T12:43:13.940' AS DateTime), 1, NULL)
INSERT [dbo].[ACCOUNT] ([AccountID], [Email], [Password], [Role], [CreatedAt], [IsActive], [Phone]) VALUES (17, N'thihau92b@gmail.com', N'$2a$11$HBf/8q/SiNzs1ln39tPPce0ElMexu5lMxqj.x3pQygDvnmA9ooF2e', N'Customer', CAST(N'2025-11-28T13:06:06.620' AS DateTime), 1, NULL)
SET IDENTITY_INSERT [dbo].[ACCOUNT] OFF
GO
SET IDENTITY_INSERT [dbo].[ADDRESS] ON 

INSERT [dbo].[ADDRESS] ([AddressID], [CustomerID], [Street], [District], [City], [Country], [PostalCode], [IsDefault]) VALUES (1, 1, N'123 Đường Lê Lợi', N'Quận 1', N'TP. Hồ Chí Minh', N'Việt Nam', NULL, 1)
INSERT [dbo].[ADDRESS] ([AddressID], [CustomerID], [Street], [District], [City], [Country], [PostalCode], [IsDefault]) VALUES (2, 2, N'789 Đường Hai Bà Trưng', N'Quận Hoàn Kiếm', N'Hà Nội', N'Việt Nam', NULL, 1)
INSERT [dbo].[ADDRESS] ([AddressID], [CustomerID], [Street], [District], [City], [Country], [PostalCode], [IsDefault]) VALUES (3, 3, N'300 Đường Nguyễn Văn Cừ', N'Quận 5', N'TP. Hồ Chí Minh', N'Việt Nam', NULL, 1)
INSERT [dbo].[ADDRESS] ([AddressID], [CustomerID], [Street], [District], [City], [Country], [PostalCode], [IsDefault]) VALUES (4, 4, N'100 Phố Hàng Bài', N'Quận Hoàn Kiếm', N'Hà Nội', N'Việt Nam', NULL, 1)
INSERT [dbo].[ADDRESS] ([AddressID], [CustomerID], [Street], [District], [City], [Country], [PostalCode], [IsDefault]) VALUES (5, 5, N'200 Đường Phạm Hùng', N'Quận Cầu Giấy', N'Hà Nội', N'Việt Nam', NULL, 1)
INSERT [dbo].[ADDRESS] ([AddressID], [CustomerID], [Street], [District], [City], [Country], [PostalCode], [IsDefault]) VALUES (6, 9, N'nguyen thi b, Phường Đồng Xuân', N'Quận Hoàn Kiếm', N'Thành phố Hà Nội', N'Việt Nam', N'700000', 1)
INSERT [dbo].[ADDRESS] ([AddressID], [CustomerID], [Street], [District], [City], [Country], [PostalCode], [IsDefault]) VALUES (7, 10, N'nguyen thi b, Phường Ngọc Thụy', N'Quận Long Biên', N'Thành phố Hà Nội', N'Việt Nam', N'700000', 1)
INSERT [dbo].[ADDRESS] ([AddressID], [CustomerID], [Street], [District], [City], [Country], [PostalCode], [IsDefault]) VALUES (1007, 13, N'nguyen thi b, Xã Na Khê', N'Huyện Yên Minh', N'Tỉnh Hà Giang', N'Việt Nam', N'700000', 1)
INSERT [dbo].[ADDRESS] ([AddressID], [CustomerID], [Street], [District], [City], [Country], [PostalCode], [IsDefault]) VALUES (1008, 15, N'123, Phường Tam Thanh', N'Thành phố Lạng Sơn', N'Tỉnh Lạng Sơn', N'Việt Nam', N'700000', 1)
INSERT [dbo].[ADDRESS] ([AddressID], [CustomerID], [Street], [District], [City], [Country], [PostalCode], [IsDefault]) VALUES (1009, 13, N'24sgywh, Xã A Lù', N'Huyện Bát Xát', N'Tỉnh Lào Cai', N'Việt Nam', N'700000', 0)
SET IDENTITY_INSERT [dbo].[ADDRESS] OFF
GO
SET IDENTITY_INSERT [dbo].[Brands] ON 

INSERT [dbo].[Brands] ([BrandId], [BrandName]) VALUES (1, N'Apple')
INSERT [dbo].[Brands] ([BrandId], [BrandName]) VALUES (2, N'Samsung')
INSERT [dbo].[Brands] ([BrandId], [BrandName]) VALUES (3, N'Google')
INSERT [dbo].[Brands] ([BrandId], [BrandName]) VALUES (4, N'Xiaomi')
INSERT [dbo].[Brands] ([BrandId], [BrandName]) VALUES (5, N'OPPO')
SET IDENTITY_INSERT [dbo].[Brands] OFF
GO
SET IDENTITY_INSERT [dbo].[CartItems] ON 

INSERT [dbo].[CartItems] ([CartItemId], [CustomerID], [VariantId], [Quantity], [CreatedDate]) VALUES (5, 1, 13, 1, CAST(N'2025-11-23T23:15:53.270' AS DateTime))
INSERT [dbo].[CartItems] ([CartItemId], [CustomerID], [VariantId], [Quantity], [CreatedDate]) VALUES (1007, 1, 2, 1, CAST(N'2025-11-28T10:41:35.023' AS DateTime))
SET IDENTITY_INSERT [dbo].[CartItems] OFF
GO
SET IDENTITY_INSERT [dbo].[CUSTOMER] ON 

INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (1, 2, N'Nguyễn Văn A', N'0901234567', N'Nam', CAST(N'1995-01-15' AS Date), N'VIP')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (2, 3, N'Trần Thị B', N'0912345678', N'Nữ', CAST(N'2000-05-20' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (3, 4, N'Phạm Minh C', N'0933445566', N'Nam', CAST(N'1998-03-01' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (4, 5, N'Lê Thị Dung D', N'0987654321', N'Nữ', CAST(N'1992-11-11' AS Date), N'VIP')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (5, 6, N'Ngô Văn Tranh H', N'0977889900', N'Nam', CAST(N'2001-07-25' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (6, 7, N'Van Toan', N'12234556', N'Nam', CAST(N'2025-11-03' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (7, 8, N'Van Toan', N'12234556666', N'Nam', CAST(N'2025-11-03' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (8, 9, N'Trung', N'0999999999', N'Nam', CAST(N'2005-07-20' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (9, 11, N'Trung', N'0999999997', N'Nam', CAST(N'2005-07-20' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (10, 12, N'Trang', N'078650135', N'Nữ', CAST(N'2005-07-20' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (11, 13, N'Nguyen Thi Bình2', N'78650135', N'Nam', CAST(N'2003-06-11' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (12, 14, N'Nguyen Thi Bình3', N'78650135000', N'Nam', CAST(N'2003-06-11' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (13, 15, N'Nguyen Van A', N'0974550987', N'Nam', CAST(N'2005-02-03' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (14, 16, N'Nguyen Van A', N'09745509872', N'Nam', CAST(N'2005-02-03' AS Date), N'Thường')
INSERT [dbo].[CUSTOMER] ([CustomerID], [AccountID], [FullName], [Phone], [Gender], [BirthDate], [CustomerType]) VALUES (15, 17, N'mimi', N'0123456789', N'Nam', CAST(N'2005-02-03' AS Date), N'Thường')
SET IDENTITY_INSERT [dbo].[CUSTOMER] OFF
GO
SET IDENTITY_INSERT [dbo].[FavoriteDetails] ON 

INSERT [dbo].[FavoriteDetails] ([FavoriteDetailId], [FavoriteId], [VariantId]) VALUES (4, 2, 16)
INSERT [dbo].[FavoriteDetails] ([FavoriteDetailId], [FavoriteId], [VariantId]) VALUES (5, 1, 17)
INSERT [dbo].[FavoriteDetails] ([FavoriteDetailId], [FavoriteId], [VariantId]) VALUES (6, 3, 4)
INSERT [dbo].[FavoriteDetails] ([FavoriteDetailId], [FavoriteId], [VariantId]) VALUES (7, 4, 1)
INSERT [dbo].[FavoriteDetails] ([FavoriteDetailId], [FavoriteId], [VariantId]) VALUES (8, 4, 21)
INSERT [dbo].[FavoriteDetails] ([FavoriteDetailId], [FavoriteId], [VariantId]) VALUES (1008, 5, 6)
INSERT [dbo].[FavoriteDetails] ([FavoriteDetailId], [FavoriteId], [VariantId]) VALUES (1009, 6, 1)
INSERT [dbo].[FavoriteDetails] ([FavoriteDetailId], [FavoriteId], [VariantId]) VALUES (1010, 6, 4)
SET IDENTITY_INSERT [dbo].[FavoriteDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[Favorites] ON 

INSERT [dbo].[Favorites] ([FavoriteId], [CustomerID], [CreatedAt]) VALUES (1, 1, CAST(N'2025-11-20T21:26:01.813' AS DateTime))
INSERT [dbo].[Favorites] ([FavoriteId], [CustomerID], [CreatedAt]) VALUES (2, 7, CAST(N'2025-11-20T23:14:11.057' AS DateTime))
INSERT [dbo].[Favorites] ([FavoriteId], [CustomerID], [CreatedAt]) VALUES (3, 9, CAST(N'2025-11-23T03:20:33.633' AS DateTime))
INSERT [dbo].[Favorites] ([FavoriteId], [CustomerID], [CreatedAt]) VALUES (4, 10, CAST(N'2025-11-23T23:18:48.047' AS DateTime))
INSERT [dbo].[Favorites] ([FavoriteId], [CustomerID], [CreatedAt]) VALUES (5, 13, CAST(N'2025-11-28T12:32:57.607' AS DateTime))
INSERT [dbo].[Favorites] ([FavoriteId], [CustomerID], [CreatedAt]) VALUES (6, 14, CAST(N'2025-11-28T12:46:56.830' AS DateTime))
INSERT [dbo].[Favorites] ([FavoriteId], [CustomerID], [CreatedAt]) VALUES (7, 15, CAST(N'2025-11-28T13:06:49.693' AS DateTime))
SET IDENTITY_INSERT [dbo].[Favorites] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderDetails] ON 

INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1, 1, 2, 1, CAST(33990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (2, 2, 4, 1, CAST(33490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (3, 2, 6, 1, CAST(25990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (4, 3, 1, 1, CAST(34990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (5, 4, 4, 1, CAST(33490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (6, 5, 3, 1, CAST(40990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (7, 6, 4, 1, CAST(33490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (8, 7, 6, 1, CAST(25990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (9, 8, 2, 1, CAST(33990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (10, 8, 5, 1, CAST(36000000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (11, 9, 1, 3, CAST(34990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (12, 10, 2, 1, CAST(33990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (13, 11, 5, 2, CAST(36000000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (14, 12, 3, 1, CAST(40990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (15, 13, 4, 1, CAST(33490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (16, 14, 6, 1, CAST(25990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (17, 15, 6, 1, CAST(25990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (18, 16, 6, 1, CAST(25990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (19, 16, 7, 1, CAST(26490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (20, 17, 7, 1, CAST(26490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (21, 18, 6, 1, CAST(25990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (22, 19, 2, 2, CAST(33990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (23, 20, 2, 1, CAST(33990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (24, 21, 6, 1, CAST(25990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (25, 22, 3, 1, CAST(40990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (26, 23, 2, 1, CAST(33990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (27, 24, 11, 1, CAST(19990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1021, 1018, 4, 1, CAST(33490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1022, 1018, 2, 2, CAST(33990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1023, 1018, 3, 3, CAST(40990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1024, 1019, 2, 1, CAST(33990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1025, 1020, 4, 1, CAST(33490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1026, 1021, 1, 2, CAST(34990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1027, 1022, 7, 1, CAST(26490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1028, 1023, 6, 1, CAST(25990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1029, 1024, 17, 1, CAST(9490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1030, 1025, 6, 1, CAST(25990000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1031, 1026, 4, 1, CAST(33490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1032, 1026, 5, 1, CAST(36000000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1033, 1027, 14, 48, CAST(13490000.00 AS Decimal(18, 2)))
INSERT [dbo].[OrderDetails] ([OrderDetailId], [OrderId], [VariantId], [Quantity], [UnitPrice]) VALUES (1034, 1028, 14, 2, CAST(13490000.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[OrderDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1, 1, CAST(N'2025-10-15T10:30:00.000' AS DateTime), N'Đã giao hàng', CAST(33990000.00 AS Decimal(18, 2)), N'Nguyễn Văn A', N'0901234567', N'123 Đường Lê Lợi', N'Quận 1', N'TP. Hồ Chí Minh', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (2, 2, CAST(N'2025-10-19T14:00:00.000' AS DateTime), N'Đang xử lý', CAST(59480000.00 AS Decimal(18, 2)), N'Trần Thị B', N'0912345678', N'789 Đường Hai Bà Trưng', N'Quận Hoàn Kiếm', N'Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (3, 1, CAST(N'2025-11-19T09:00:00.000' AS DateTime), N'Chờ xác nhận', CAST(34990000.00 AS Decimal(18, 2)), N'Nguyễn Văn A', N'0901234567', N'123 Đường Lê Lợi', N'Quận 1', N'TP. Hồ Chí Minh', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (4, 3, CAST(N'2025-11-19T09:30:00.000' AS DateTime), N'Chờ xác nhận', CAST(33490000.00 AS Decimal(18, 2)), N'Phạm Minh C', N'0933445566', N'300 Đường Nguyễn Văn Cừ', N'Quận 5', N'TP. Hồ Chí Minh', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (5, 4, CAST(N'2025-11-20T10:00:00.000' AS DateTime), N'Đang xử lý', CAST(40990000.00 AS Decimal(18, 2)), N'Lê Thị Dung D', N'0987654321', N'100 Phố Hàng Bài', N'Quận Hoàn Kiếm', N'Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (6, 2, CAST(N'2025-11-20T11:30:00.000' AS DateTime), N'Chờ xác nhận', CAST(33490000.00 AS Decimal(18, 2)), N'Trần Thị B', N'0912345678', N'789 Đường Hai Bà Trưng', N'Quận Hoàn Kiếm', N'Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (7, 1, CAST(N'2025-11-20T12:00:00.000' AS DateTime), N'Đang xử lý', CAST(25990000.00 AS Decimal(18, 2)), N'Nguyễn Văn A', N'0901234567', N'123 Đường Lê Lợi', N'Quận 1', N'TP. Hồ Chí Minh', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (8, 3, CAST(N'2025-11-20T14:15:00.000' AS DateTime), N'Chờ xác nhận', CAST(79990000.00 AS Decimal(18, 2)), N'Phạm Minh C', N'0933445566', N'300 Đường Nguyễn Văn Cừ', N'Quận 5', N'TP. Hồ Chí Minh', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (9, 4, CAST(N'2025-11-21T09:30:00.000' AS DateTime), N'Đang xử lý', CAST(104970000.00 AS Decimal(18, 2)), N'Lê Thị Dung D', N'0987654321', N'100 Phố Hàng Bài', N'Quận Hoàn Kiếm', N'Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (10, 5, CAST(N'2025-11-21T11:00:00.000' AS DateTime), N'Đã giao', CAST(33990000.00 AS Decimal(18, 2)), N'Ngô Văn Tranh H', N'0977889900', N'200 Đường Phạm Hùng', N'Quận Cầu Giấy', N'Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (11, 1, CAST(N'2025-11-21T15:45:00.000' AS DateTime), N'Đã giao', CAST(72000000.00 AS Decimal(18, 2)), N'Nguyễn Văn A', N'0901234567', N'123 Đường Lê Lợi', N'Quận 1', N'TP. Hồ Chí Minh', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (12, 2, CAST(N'2025-11-22T08:30:00.000' AS DateTime), N'Đã giao', CAST(40990000.00 AS Decimal(18, 2)), N'Trần Thị B', N'0912345678', N'789 Đường Hai Bà Trưng', N'Quận Hoàn Kiếm', N'Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (13, 3, CAST(N'2025-11-22T10:10:00.000' AS DateTime), N'Đang giao', CAST(33490000.00 AS Decimal(18, 2)), N'Phạm Minh C', N'0933445566', N'300 Đường Nguyễn Văn Cừ', N'Quận 5', N'TP. Hồ Chí Minh', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (14, 4, CAST(N'2025-11-22T11:50:00.000' AS DateTime), N'Đã hủy', CAST(25990000.00 AS Decimal(18, 2)), N'Lê Thị Dung D', N'0987654321', N'100 Phố Hàng Bài', N'Quận Hoàn Kiếm', N'Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (15, 9, CAST(N'2025-11-23T10:43:23.220' AS DateTime), N'Chờ xác nhận', CAST(25990000.00 AS Decimal(18, 2)), N'Trung', N'0999999997', N'nguyen thi b, Phường Đồng Xuân', N'Quận Hoàn Kiếm', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (16, 9, CAST(N'2025-11-23T10:45:12.773' AS DateTime), N'Đã giao', CAST(52480000.00 AS Decimal(18, 2)), N'Trung', N'0999999997', N'nguyen thi b, Phường Đồng Xuân', N'Quận Hoàn Kiếm', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (17, 9, CAST(N'2025-11-23T15:34:43.660' AS DateTime), N'Đang giao', CAST(26490000.00 AS Decimal(18, 2)), N'Trung', N'0999999997', N'nguyen thi b, Phường Đồng Xuân', N'Quận Hoàn Kiếm', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (18, 10, CAST(N'2025-11-27T01:36:37.197' AS DateTime), N'Chờ xác nhận', CAST(25990000.00 AS Decimal(18, 2)), N'Trang', N'078650135', N'nguyen thi b, Phường Ngọc Thụy', N'Quận Long Biên', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (19, 10, CAST(N'2025-11-27T01:39:20.280' AS DateTime), N'Đã hủy', CAST(67980000.00 AS Decimal(18, 2)), N'Trang', N'078650135', N'nguyen thi b, Phường Ngọc Thụy', N'Quận Long Biên', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (20, 10, CAST(N'2025-11-27T01:55:37.573' AS DateTime), N'Chờ xác nhận', CAST(33990000.00 AS Decimal(18, 2)), N'Trang', N'078650135', N'nguyen thi b, Phường Ngọc Thụy', N'Quận Long Biên', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (21, 10, CAST(N'2025-11-27T02:11:22.193' AS DateTime), N'Đã hủy', CAST(25990000.00 AS Decimal(18, 2)), N'Trang', N'078650135', N'nguyen thi b, Phường Ngọc Thụy', N'Quận Long Biên', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (22, 10, CAST(N'2025-11-27T02:15:22.677' AS DateTime), N'Đã giao', CAST(40990000.00 AS Decimal(18, 2)), N'Trang', N'078650135', N'nguyen thi b, Phường Ngọc Thụy', N'Quận Long Biên', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (23, 10, CAST(N'2025-11-27T10:44:55.323' AS DateTime), N'Đã hủy', CAST(33990000.00 AS Decimal(18, 2)), N'Trang', N'078650135', N'nguyen thi b, Phường Ngọc Thụy', N'Quận Long Biên', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (24, 10, CAST(N'2025-11-27T10:45:08.420' AS DateTime), N'Đã giao', CAST(19990000.00 AS Decimal(18, 2)), N'Trang', N'078650135', N'nguyen thi b, Phường Ngọc Thụy', N'Quận Long Biên', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1018, 10, CAST(N'2025-11-28T10:48:51.027' AS DateTime), N'Đã hủy', CAST(224440000.00 AS Decimal(18, 2)), N'Trang', N'078650135', N'nguyen thi b, Phường Ngọc Thụy', N'Quận Long Biên', N'Thành phố Hà Nội', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1019, 13, CAST(N'2025-11-28T12:32:19.643' AS DateTime), N'Đã hủy', CAST(33990000.00 AS Decimal(18, 2)), N'Nguyen Van A', N'0974550987', N'nguyen thi b, Xã Na Khê', N'Huyện Yên Minh', N'Tỉnh Hà Giang', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1020, 13, CAST(N'2025-11-28T12:32:41.080' AS DateTime), N'Đang giao', CAST(33490000.00 AS Decimal(18, 2)), N'Nguyen Van A', N'0974550987', N'nguyen thi b, Xã Na Khê', N'Huyện Yên Minh', N'Tỉnh Hà Giang', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1021, 15, CAST(N'2025-11-28T13:08:06.520' AS DateTime), N'Đang giao', CAST(69980000.00 AS Decimal(18, 2)), N'mimi', N'0123456789', N'123, Phường Tam Thanh', N'Thành phố Lạng Sơn', N'Tỉnh Lạng Sơn', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1022, 13, CAST(N'2025-11-28T14:25:21.113' AS DateTime), N'Đã giao', CAST(26490000.00 AS Decimal(18, 2)), N'Nguyen Van A', N'0974550987', N'nguyen thi b, Xã Na Khê', N'Huyện Yên Minh', N'Tỉnh Hà Giang', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1023, 13, CAST(N'2025-12-03T09:37:29.930' AS DateTime), N'Đã hủy', CAST(25990000.00 AS Decimal(18, 2)), N'Nguyen Van A', N'0974550987', N'nguyen thi b, Xã Na Khê', N'Huyện Yên Minh', N'Tỉnh Hà Giang', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1024, 13, CAST(N'2025-12-03T09:38:16.087' AS DateTime), N'Đã hủy', CAST(9490000.00 AS Decimal(18, 2)), N'Nguyen Van A', N'0974550987', N'nguyen thi b, Xã Na Khê', N'Huyện Yên Minh', N'Tỉnh Hà Giang', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1025, 13, CAST(N'2026-03-05T00:20:57.497' AS DateTime), N'Chờ xác nhận', CAST(25990000.00 AS Decimal(18, 2)), N'Nguyen Van A', N'0974550987', N'nguyen thi b, Xã Na Khê', N'Huyện Yên Minh', N'Tỉnh Hà Giang', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1026, 13, CAST(N'2026-03-08T09:57:05.193' AS DateTime), N'Chờ xác nhận', CAST(69490000.00 AS Decimal(18, 2)), N'Nguyen Van A', N'0974550987', N'24sgywh, Xã A Lù', N'Huyện Bát Xát', N'Tỉnh Lào Cai', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1027, 13, CAST(N'2026-03-08T15:09:25.517' AS DateTime), N'Chờ xác nhận', CAST(647520000.00 AS Decimal(18, 2)), N'Nguyen Van A', N'0974550987', N'nguyen thi b, Xã Na Khê', N'Huyện Yên Minh', N'Tỉnh Hà Giang', N'Việt Nam')
INSERT [dbo].[Orders] ([OrderId], [CustomerID], [OrderDate], [Status], [TotalAmount], [ShippingFullName], [ShippingPhone], [ShippingStreet], [ShippingDistrict], [ShippingCity], [ShippingCountry]) VALUES (1028, 13, CAST(N'2026-03-08T15:10:02.443' AS DateTime), N'Chờ xác nhận', CAST(26980000.00 AS Decimal(18, 2)), N'Nguyen Van A', N'0974550987', N'nguyen thi b, Xã Na Khê', N'Huyện Yên Minh', N'Tỉnh Hà Giang', N'Việt Nam')
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[Payments] ON 

INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1, 1, N'COD', CAST(N'2025-10-17T11:00:00.000' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (2, 2, N'VnPay', CAST(N'2025-10-19T14:01:00.000' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (3, 3, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (4, 4, N'Chuyển khoản', CAST(N'2025-11-19T09:31:00.000' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (5, 5, N'Chuyển khoản', CAST(N'2025-11-20T10:05:00.000' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (6, 6, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (7, 7, N'VnPay', CAST(N'2025-11-20T12:01:00.000' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (8, 8, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (9, 9, N'Chuyển khoản', CAST(N'2025-11-21T09:35:00.000' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (10, 10, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (11, 11, N'VnPay', CAST(N'2025-11-21T15:50:00.000' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (12, 12, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (13, 13, N'Chuyển khoản', CAST(N'2025-11-22T10:11:00.000' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (14, 14, N'VnPay', CAST(N'2025-11-22T11:51:00.000' AS DateTime), N'Hoàn tiền')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (15, 15, N'Banking', CAST(N'2025-11-23T10:43:23.343' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (16, 16, N'Banking', CAST(N'2025-11-23T10:45:12.797' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (17, 17, N'Banking', CAST(N'2025-11-23T15:34:43.797' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (18, 18, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (19, 19, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (20, 20, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (21, 21, N'Banking', CAST(N'2025-11-27T02:11:22.330' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (22, 22, N'COD', CAST(N'2025-11-28T10:51:10.687' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (23, 23, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (24, 24, N'COD', CAST(N'2025-11-28T14:20:01.543' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1018, 1018, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1019, 1019, N'Banking', CAST(N'2025-11-28T12:32:19.757' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1020, 1020, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1021, 1021, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1022, 1022, N'Banking', CAST(N'2025-11-28T14:25:21.137' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1023, 1023, N'Banking', CAST(N'2025-12-03T09:37:31.387' AS DateTime), N'Đã thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1024, 1024, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1025, 1025, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1026, 1026, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1027, 1027, N'COD', NULL, N'Chưa thanh toán')
INSERT [dbo].[Payments] ([PaymentId], [OrderId], [PaymentMethod], [PaymentDate], [PaymentStatus]) VALUES (1028, 1028, N'COD', NULL, N'Chưa thanh toán')
SET IDENTITY_INSERT [dbo].[Payments] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (1, N'iPhone 15 Pro Max', 1, N'Apple A17 Pro', N'iOS 17', 4422, 0, CAST(6.70 AS Decimal(4, 2)), N'Super Retina XDR OLED', 120, N'Chính 48MP & Siêu rộng 12MP & Tele 12MP', N'12MP', CAST(221.00 AS Decimal(5, 2)), N'159.9 x 76.7 x 8.3 mm', N'Khung Titan, USB-C, Nút Action Button mới.', CAST(N'2024-09-22' AS Date), N'/images/iphone-15-pro-max.jpg', 1, CAST(N'2025-11-20T20:04:19.423' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (2, N'Samsung Galaxy S24 Ultra', 2, N'Snapdragon 8 Gen 3', N'Android 14', 5000, 1, CAST(6.80 AS Decimal(4, 2)), N'Dynamic AMOLED 2X', 120, N'Chính 200MP & Siêu rộng 12MP & Tele 50MP (5x) & Tele 10MP (3x)', N'12MP', CAST(232.00 AS Decimal(5, 2)), N'162.3 x 79 x 8.6 mm', N'Tích hợp S Pen, Khung Titan, Galaxy AI.', CAST(N'2025-01-17' AS Date), N'/images/s24-ultra.jpg', 1, CAST(N'2025-11-20T20:04:19.423' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (3, N'Google Pixel 8 Pro', 3, N'Google Tensor G3', N'Android 14', 5050, 0, CAST(6.70 AS Decimal(4, 2)), N'Super Actua LTPO OLED', 120, N'Chính 50MP & Siêu rộng 48MP & Tele 48MP', N'10.5MP', CAST(213.00 AS Decimal(5, 2)), N'162.6 x 76.5 x 8.8 mm', N'Trải nghiệm Android thuần túy, tính năng AI nhiếp ảnh độc quyền.', CAST(N'2024-10-12' AS Date), N'/images/pixel-8-pro.jpg', 1, CAST(N'2025-11-20T20:04:19.423' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (4, N'iPhone 14 Pro Max', 1, N'Apple A16 Bionic', N'iOS 16', 4323, 0, CAST(6.70 AS Decimal(4, 2)), N'Super Retina XDR OLED', 120, N'48MP & 12MP & 12MP', N'12MP', CAST(240.00 AS Decimal(5, 2)), N'160.7 x 77.6 x 7.9 mm', N'Dynamic Island đột phá, màn hình Always-On.', CAST(N'2022-09-16' AS Date), N'/images/iphone-14-pro-max.jpg', 1, CAST(N'2025-11-20T20:04:19.537' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (6, N'Xiaomi 14 5G', 4, N'Snapdragon 8 Gen 3', N'Android 14', 4610, 1, CAST(6.36 AS Decimal(4, 2)), N'LTPO OLED', 120, N'50MP & 50MP & 50MP', N'32MP', CAST(193.00 AS Decimal(5, 2)), N'152.8 x 71.5 x 8.2 mm', N'Thấu kính Leica huyền thoại, hiệu năng đỉnh cao.', CAST(N'2024-03-10' AS Date), N'/images/xiaomi-14.jpg', 1, CAST(N'2025-11-20T20:04:19.540' AS DateTime), CAST(N'2025-11-28T11:25:13.473' AS DateTime))
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (7, N'OPPO Find N3 Flip', 5, N'Dimensity 9200', N'Android 13', 4300, 1, CAST(6.80 AS Decimal(4, 2)), N'Foldable LTPO AMOLED', 120, N'50MP & 32MP & 48MP', N'32MP', CAST(198.00 AS Decimal(5, 2)), N'Gập: 85.5 x 75.8 x 16.5 mm', N'Gập mở phong cách, camera Hasselblad chuyên nghiệp.', CAST(N'2023-10-20' AS Date), N'/images/oppo-n3-flip.jpg', 1, CAST(N'2025-11-20T20:04:19.540' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (8, N'iPhone 13', 1, N'Apple A15 Bionic', N'iOS 15', 3240, 0, CAST(6.10 AS Decimal(4, 2)), N'Super Retina XDR OLED', 60, N'12MP & 12MP', N'12MP', CAST(174.00 AS Decimal(5, 2)), N'146.7 x 71.5 x 7.7 mm', N'Thiết kế bền bỉ, camera chéo độc đáo.', CAST(N'2021-09-24' AS Date), N'/images/iphone-13.jpg', 1, CAST(N'2025-11-20T20:04:19.540' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (9, N'Samsung Galaxy Z Fold5', 2, N'Snapdragon 8 Gen 2', N'Android 13', 4400, 1, CAST(7.60 AS Decimal(4, 2)), N'Foldable Dynamic AMOLED 2X', 120, N'50MP & 12MP & 10MP', N'4MP (Ẩn)', CAST(253.00 AS Decimal(5, 2)), N'Gập: 154.9 x 67.1 x 13.4 mm', N'Mở ra thế giới mới, đa nhiệm cực đỉnh.', CAST(N'2023-08-11' AS Date), N'/images/z-fold-5.jpg', 1, CAST(N'2025-11-20T20:04:19.540' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (10, N'Xiaomi Redmi Note 13 Pro+', 4, N'Dimensity 7200 Ultra', N'Android 13', 5000, 1, CAST(6.67 AS Decimal(4, 2)), N'AMOLED 1.5K', 120, N'200MP & 8MP & 2MP', N'16MP', CAST(204.00 AS Decimal(5, 2)), N'161.4 x 74.2 x 8.9 mm', N'Camera 200MP, sạc nhanh 120W siêu tốc.', CAST(N'2024-01-15' AS Date), N'/images/redmi-note-13.jpg', 1, CAST(N'2025-11-20T20:04:19.540' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (11, N'iPhone 15 Plus', 1, N'Apple A16 Bionic', N'iOS 17', 4383, 0, CAST(6.70 AS Decimal(4, 2)), N'Super Retina XDR OLED', 60, N'48MP & 12MP', N'12MP', CAST(201.00 AS Decimal(5, 2)), N'160.9 x 77.8 x 7.8 mm', N'Màn hình lớn, pin siêu trâu, Dynamic Island.', CAST(N'2023-09-22' AS Date), N'/images/iphone-15-plus.jpg', 1, CAST(N'2025-11-20T20:04:19.540' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (12, N'Samsung Galaxy A55 5G', 2, N'Exynos 1480', N'Android 14', 5000, 0, CAST(6.60 AS Decimal(4, 2)), N'Super AMOLED', 120, N'50MP & 12MP & 5MP', N'32MP', CAST(213.00 AS Decimal(5, 2)), N'161.1 x 77.4 x 8.2 mm', N'Khung viền kim loại, thiết kế Key Island.', CAST(N'2024-03-11' AS Date), N'/images/samsung-a55.jpg', 1, CAST(N'2025-11-20T20:04:19.540' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (13, N'OPPO Reno11 F 5G', 5, N'Dimensity 7050', N'Android 14', 5000, 1, CAST(6.70 AS Decimal(4, 2)), N'AMOLED', 120, N'64MP & 8MP & 2MP', N'32MP', CAST(177.00 AS Decimal(5, 2)), N'161.1 x 74.7 x 7.5 mm', N'Chuyên gia chân dung, thiết kế vân đá.', CAST(N'2024-02-28' AS Date), N'/images/oppo-reno11f.jpg', 1, CAST(N'2025-11-20T20:04:19.540' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (15, N'Xiaomi note 12 Pro', 1, N'Google Tensor G3', N'android', 2, 1, CAST(8.00 AS Decimal(4, 2)), N'2', 2, N'2', N'2', CAST(2.00 AS Decimal(5, 2)), N'2', NULL, CAST(N'2025-10-29' AS Date), N'/images/products/d768a9e6-d938-460b-ade5-bc4c25495754_xiaomi note 12 Pro.jpg', 0, CAST(N'2025-11-28T11:33:11.700' AS DateTime), CAST(N'2025-12-07T21:14:48.087' AS DateTime))
INSERT [dbo].[Products] ([ProductId], [Name], [BrandId], [Chipset], [OperatingSystem], [BatteryCapacity], [ChargerIncluded], [ScreenSize], [ScreenTech], [RefreshRate], [RearCamera], [FrontCamera], [Weight], [Dimensions], [Description], [ReleaseDate], [MainImage], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (17, N'Xiaomi note 12 Pro', 4, N'Google Tensor G3', N'Android 14', 5000, 1, CAST(6.70 AS Decimal(4, 2)), N'Super Actua LTPO OLED', 120, N'Chính 50MP & Siêu rộng 48MP & Tele 48MP', N'10.5MP', CAST(196.00 AS Decimal(5, 2)), N'162.6 x 76.5 x 8.8 mm', NULL, CAST(N'2025-10-27' AS Date), N'/images/products/d42c5ae2-0a44-4a97-b6db-28173c6d58fa_xiaomi.jpg', 0, CAST(N'2025-11-28T14:08:47.220' AS DateTime), CAST(N'2025-11-28T14:11:29.137' AS DateTime))
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
SET IDENTITY_INSERT [dbo].[ProductVariants] ON 

INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (1, 1, N'Titan Tự Nhiên', N'256GB', N'8GB', CAST(34990000.00 AS Decimal(18, 2)), NULL, 46, N'/images/iphone-15-pro-titan.jpg', 1, CAST(N'2025-11-20T20:04:19.433' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (2, 1, N'Titan Xanh', N'256GB', N'8GB', CAST(34990000.00 AS Decimal(18, 2)), CAST(33990000.00 AS Decimal(18, 2)), 26, N'/images/iphone-15-pro-blue.jpg', 1, CAST(N'2025-11-20T20:04:19.433' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (3, 1, N'Titan Trắng', N'512GB', N'8GB', CAST(40990000.00 AS Decimal(18, 2)), NULL, 17, N'/images/iphone-15-pro-white.jpg', 1, CAST(N'2025-11-20T20:04:19.433' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (4, 2, N'Xám Titan', N'256GB', N'12GB', CAST(33490000.00 AS Decimal(18, 2)), NULL, 37, N'/images/s24-ultra-gray.jpg', 1, CAST(N'2025-11-20T20:04:19.433' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (5, 2, N'Tím Titan', N'512GB', N'12GB', CAST(37490000.00 AS Decimal(18, 2)), CAST(36000000.00 AS Decimal(18, 2)), 12, N'/images/s24-ultra-violet.jpg', 1, CAST(N'2025-11-20T20:04:19.433' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (6, 3, N'Obsidian (Đen)', N'128GB', N'12GB', CAST(25990000.00 AS Decimal(18, 2)), NULL, 20, N'/images/pixel-8-pro-obsidian.jpg', 1, CAST(N'2025-11-20T20:04:19.433' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (7, 4, N'Tím Đậm', N'128GB', N'6GB', CAST(27990000.00 AS Decimal(18, 2)), CAST(26490000.00 AS Decimal(18, 2)), 9, N'/images/iphone-14-pro-max-tim.jpg', 1, CAST(N'2025-11-20T20:04:19.620' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (8, 4, N'Vàng Gold', N'256GB', N'6GB', CAST(29990000.00 AS Decimal(18, 2)), NULL, 10, N'/images/iphone-14-pro-max-vang.jpg', 1, CAST(N'2025-11-20T20:04:19.620' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (11, 6, N'Xanh Lá', N'256GB', N'12GB', CAST(19990000.00 AS Decimal(18, 2)), NULL, 28, N'/images/xiaomi-14-green.jpg', 1, CAST(N'2025-11-20T20:04:19.623' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (12, 6, N'Đen', N'512GB', N'12GB', CAST(22490000.00 AS Decimal(18, 2)), CAST(21490000.00 AS Decimal(18, 2)), 10, N'/images/xiaomi-14-black.jpg', 1, CAST(N'2025-11-20T20:04:19.623' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (13, 7, N'Vàng Đồng', N'256', N'12', CAST(22990000.00 AS Decimal(18, 2)), CAST(2000000.00 AS Decimal(18, 2)), 20, N'/images/oppo-n3-flip-gold.jpg', 1, CAST(N'2025-11-20T20:04:19.623' AS DateTime), CAST(N'2025-11-28T09:04:21.407' AS DateTime))
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (14, 8, N'Hồng', N'128GB', N'4GB', CAST(13990000.00 AS Decimal(18, 2)), CAST(13490000.00 AS Decimal(18, 2)), 0, N'/images/iphone-13-pink.jpg', 1, CAST(N'2025-11-20T20:04:19.623' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (15, 8, N'Xanh Dương', N'128GB', N'4GB', CAST(13990000.00 AS Decimal(18, 2)), NULL, 40, N'/images/iphone-13-blue.jpg', 1, CAST(N'2025-11-20T20:04:19.623' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (16, 9, N'Xanh Icy', N'256GB', N'12GB', CAST(36990000.00 AS Decimal(18, 2)), CAST(34990000.00 AS Decimal(18, 2)), 10, N'/images/z-fold-5-blue.jpg', 1, CAST(N'2025-11-20T20:04:19.623' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (17, 10, N'Tím Cực Quang', N'256GB', N'8GB', CAST(9490000.00 AS Decimal(18, 2)), NULL, 60, N'/images/redmi-note-13-purple.jpg', 1, CAST(N'2025-11-20T20:04:19.623' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (18, 11, N'Xanh Lá', N'128GB', N'6GB', CAST(22990000.00 AS Decimal(18, 2)), NULL, 30, N'/images/iphone-15-plus-green.jpg', 1, CAST(N'2025-11-20T20:04:19.627' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (19, 11, N'Vàng', N'256GB', N'6GB', CAST(25990000.00 AS Decimal(18, 2)), NULL, 20, N'/images/iphone-15-plus-yellow.jpg', 1, CAST(N'2025-11-20T20:04:19.627' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (20, 12, N'Tím Lilac', N'128GB', N'8GB', CAST(9690000.00 AS Decimal(18, 2)), NULL, 50, N'/images/samsung-a55-purple.jpg', 1, CAST(N'2025-11-20T20:04:19.627' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (21, 13, N'Xanh Dương', N'256GB', N'8GB', CAST(8490000.00 AS Decimal(18, 2)), NULL, 40, N'/images/oppo-reno11f-blue.jpg', 1, CAST(N'2025-11-20T20:04:19.627' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (23, 15, N'trắng', N'256GB', N'8GB', CAST(18000000.00 AS Decimal(18, 2)), CAST(15000000.00 AS Decimal(18, 2)), 70, N'/images/products/d768a9e6-d938-460b-ade5-bc4c25495754_xiaomi note 12 Pro.jpg', 1, CAST(N'2025-11-28T11:33:12.030' AS DateTime), CAST(N'2025-11-28T12:52:49.940' AS DateTime))
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (25, 15, N'xanh dương', N'128GB', N'8GB', CAST(15000000.00 AS Decimal(18, 2)), CAST(10000000.00 AS Decimal(18, 2)), 20, N'/images/products/574635fd-a9ec-4e9b-baf5-4495c39c440e_xiaomi.jpg', 1, CAST(N'2025-11-28T12:49:58.003' AS DateTime), NULL)
INSERT [dbo].[ProductVariants] ([VariantId], [ProductId], [Color], [Storage], [RAM], [Price], [DiscountPrice], [Stock], [ImageUrl], [IsActive], [CreatedDate], [UpdatedDate]) VALUES (26, 17, N'trắng', N'256GB', N'8GB', CAST(15000000.00 AS Decimal(18, 2)), CAST(13000000.00 AS Decimal(18, 2)), 40, N'/images/products/d42c5ae2-0a44-4a97-b6db-28173c6d58fa_xiaomi.jpg', 1, CAST(N'2025-11-28T14:08:47.257' AS DateTime), CAST(N'2025-11-28T14:11:13.277' AS DateTime))
SET IDENTITY_INSERT [dbo].[ProductVariants] OFF
GO
SET IDENTITY_INSERT [dbo].[Shipping] ON 

INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (1, 1, N'Giao Hàng Nhanh', N'GHN123456ABC', CAST(N'2025-10-15T17:00:00.000' AS DateTime), CAST(N'2025-10-17T00:00:00.000' AS DateTime), CAST(N'2025-10-17T10:55:00.000' AS DateTime), N'Đã giao', N'Khách hàng đã nhận hàng.')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (2, 13, N'Viettel Post', N'VTPOST999TEST', CAST(N'2025-11-22T10:30:00.000' AS DateTime), CAST(N'2025-11-25T00:00:00.000' AS DateTime), NULL, N'Đang vận chuyển', N'Hàng đang trên đường đến bưu cục gần nhất.')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (3, 12, N'Viettel Post', N'VT122111', CAST(N'2025-11-21T10:23:25.683' AS DateTime), CAST(N'2025-11-24T10:23:25.683' AS DateTime), CAST(N'2025-11-21T10:23:31.630' AS DateTime), N'Đã giao', N'Bắt đầu vận chuyển đơn hàng #12')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (4, 11, N'Viettel Post', N'VT112211', CAST(N'2025-11-22T16:08:01.407' AS DateTime), CAST(N'2025-11-25T16:08:01.410' AS DateTime), CAST(N'2025-11-22T16:08:05.007' AS DateTime), N'Đã giao', N'Bắt đầu vận chuyển đơn hàng #11')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (5, 10, N'Viettel Post', N'VT102211', CAST(N'2025-11-22T21:03:48.497' AS DateTime), CAST(N'2025-11-25T21:03:48.497' AS DateTime), CAST(N'2025-11-22T21:03:53.050' AS DateTime), N'Đã giao', N'Bắt đầu vận chuyển đơn hàng #10')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (6, 16, N'Giao Hàng Nhanh', N'GHN231116', CAST(N'2025-11-23T10:46:08.570' AS DateTime), CAST(N'2025-11-26T00:00:00.000' AS DateTime), CAST(N'2025-11-23T10:46:13.520' AS DateTime), N'Giao thành công', N'Đã bàn giao cho Giao Hàng Nhanh')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (7, 17, N'Giao Hàng Tiết Kiệm', N'GHTK231117', CAST(N'2025-11-23T15:35:30.953' AS DateTime), CAST(N'2025-11-26T00:00:00.000' AS DateTime), NULL, N'Đang vận chuyển', N'Đã bàn giao cho Giao Hàng Tiết Kiệm')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (8, 22, N'Giao Hàng Nhanh', N'GHN281122', CAST(N'2025-11-28T10:51:04.900' AS DateTime), CAST(N'2025-12-01T00:00:00.000' AS DateTime), CAST(N'2025-11-28T10:51:10.687' AS DateTime), N'Giao thành công', N'Đã bàn giao cho Giao Hàng Nhanh')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (9, 1021, N'Giao Hàng Nhanh', N'GHN28111021', CAST(N'2025-11-28T13:58:40.910' AS DateTime), CAST(N'2025-12-01T00:00:00.000' AS DateTime), NULL, N'Đang vận chuyển', N'Đã bàn giao cho Giao Hàng Nhanh')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (10, 1020, N'Giao Hàng Tiết Kiệm', N'GHTK28111020', CAST(N'2025-11-28T13:59:31.633' AS DateTime), CAST(N'2025-12-01T00:00:00.000' AS DateTime), NULL, N'Đang vận chuyển', N'Đã bàn giao cho Giao Hàng Tiết Kiệm')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (11, 24, N'Viettel Post', N'VT281124', CAST(N'2025-11-28T14:19:34.190' AS DateTime), CAST(N'2025-12-01T00:00:00.000' AS DateTime), CAST(N'2025-11-28T14:20:01.543' AS DateTime), N'Giao thành công', N'Đã bàn giao cho Viettel Post')
INSERT [dbo].[Shipping] ([ShippingId], [OrderId], [Carrier], [TrackingNumber], [ShippedDate], [EstimatedDelivery], [DeliveredDate], [Status], [Note]) VALUES (12, 1022, N'Viettel Post', N'VT07121022', CAST(N'2025-12-07T21:15:20.903' AS DateTime), CAST(N'2025-12-10T00:00:00.000' AS DateTime), CAST(N'2025-12-07T21:15:24.710' AS DateTime), N'Giao thành công', N'Đã bàn giao cho Viettel Post')
SET IDENTITY_INSERT [dbo].[Shipping] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__ACCOUNT__A9D105341E7A5892]    Script Date: 3/8/2026 3:57:49 PM ******/
ALTER TABLE [dbo].[ACCOUNT] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ACCOUNT] ADD  DEFAULT ('Customer') FOR [Role]
GO
ALTER TABLE [dbo].[ACCOUNT] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ACCOUNT] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ADDRESS] ADD  DEFAULT (N'Việt Nam') FOR [Country]
GO
ALTER TABLE [dbo].[ADDRESS] ADD  DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[CartItems] ADD  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[CartItems] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[CUSTOMER] ADD  DEFAULT ('Nam') FOR [Gender]
GO
ALTER TABLE [dbo].[CUSTOMER] ADD  DEFAULT (N'Thường') FOR [CustomerType]
GO
ALTER TABLE [dbo].[Favorites] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT ((0)) FOR [IsRead]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (N'Việt Nam') FOR [ShippingCountry]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((1)) FOR [ChargerIncluded]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ProductVariants] ADD  DEFAULT ((0)) FOR [Stock]
GO
ALTER TABLE [dbo].[ProductVariants] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ProductVariants] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Reviews] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ADDRESS]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CUSTOMER] ([CustomerID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CUSTOMER] ([CustomerID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD FOREIGN KEY([VariantId])
REFERENCES [dbo].[ProductVariants] ([VariantId])
GO
ALTER TABLE [dbo].[CUSTOMER]  WITH CHECK ADD FOREIGN KEY([AccountID])
REFERENCES [dbo].[ACCOUNT] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FavoriteDetails]  WITH CHECK ADD FOREIGN KEY([FavoriteId])
REFERENCES [dbo].[Favorites] ([FavoriteId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FavoriteDetails]  WITH CHECK ADD FOREIGN KEY([VariantId])
REFERENCES [dbo].[ProductVariants] ([VariantId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Favorites]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CUSTOMER] ([CustomerID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD FOREIGN KEY([AccountID])
REFERENCES [dbo].[ACCOUNT] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([VariantId])
REFERENCES [dbo].[ProductVariants] ([VariantId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CUSTOMER] ([CustomerID])
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brands] ([BrandId])
GO
ALTER TABLE [dbo].[ProductVariants]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[ReviewDetails]  WITH CHECK ADD FOREIGN KEY([ReviewId])
REFERENCES [dbo].[Reviews] ([ReviewId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewDetails]  WITH CHECK ADD FOREIGN KEY([VariantId])
REFERENCES [dbo].[ProductVariants] ([VariantId])
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[CUSTOMER] ([CustomerID])
GO
ALTER TABLE [dbo].[Shipping]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[ACCOUNT]  WITH CHECK ADD CHECK  (([Role]='Customer' OR [Role]='Admin'))
GO
ALTER TABLE [dbo].[CUSTOMER]  WITH CHECK ADD CHECK  (([CustomerType]=N'VIP' OR [CustomerType]=N'Thường'))
GO
ALTER TABLE [dbo].[CUSTOMER]  WITH CHECK ADD CHECK  (([Gender]=N'Khác' OR [Gender]=N'Nữ' OR [Gender]=N'Nam'))
GO
ALTER TABLE [dbo].[ReviewDetails]  WITH CHECK ADD CHECK  (([Rating]>=(1) AND [Rating]<=(5)))
GO
