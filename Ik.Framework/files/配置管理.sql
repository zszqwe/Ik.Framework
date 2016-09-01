/****** Object:  Table [dbo].[cfg_definition]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cfg_definition](
	[def_id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[title] [varchar](50) NOT NULL,
	[desc] [varchar](200) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cfg_definition_item]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cfg_definition_item](
	[id] [uniqueidentifier] NOT NULL,
	[def_ver_id] [uniqueidentifier] NULL,
	[item_ver_env_val_id] [uniqueidentifier] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cfg_definition_version]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cfg_definition_version](
	[def_ver_id] [uniqueidentifier] NOT NULL,
	[def_id] [uniqueidentifier] NOT NULL,
	[value] [varchar](10) NOT NULL,
	[desc] [varchar](200) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cfg_environment]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cfg_environment](
	[env_id] [uniqueidentifier] NOT NULL,
	[name] [varchar](20) NULL,
	[key] [varchar](20) NULL,
	[desc] [varchar](200) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cfg_item]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cfg_item](
	[item_id] [uniqueidentifier] NOT NULL,
	[key] [varchar](20) NULL,
	[desc] [varchar](200) NULL,
	[item_type] [int] NULL,
	[def_id] [uniqueidentifier] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cfg_version_environment_item]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cfg_version_environment_item](
	[item_ver_env_val_id] [uniqueidentifier] NOT NULL,
	[item_ver_id] [uniqueidentifier] NOT NULL,
	[env_id] [uniqueidentifier] NOT NULL,
	[value] [varchar](max) NOT NULL,
	[desc] [varchar](200) NULL,
	[update_time] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cfg_version_item]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cfg_version_item](
	[item_ver_id] [uniqueidentifier] NOT NULL,
	[item_id] [uniqueidentifier] NULL,
	[ver] [varchar](10) NULL,
	[desc] [varchar](200) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
