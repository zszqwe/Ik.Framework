GO
/****** Object:  Table [dbo].[auth_app_info]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[auth_app_info](
	[app_id] [uniqueidentifier] NOT NULL,
	[code] [varchar](50) NULL,
	[name] [varchar](50) NULL,
	[desc] [varchar](200) NULL,
	[is_enable] [bit] NULL,
	[is_delete] [bit] NULL,
	[create_time] [datetime] NULL,
	[update_time] [datetime] NULL,
 CONSTRAINT [PK_auth_app_info] PRIMARY KEY CLUSTERED 
(
	[app_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[auth_function_info]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[auth_function_info](
	[function_id] [uniqueidentifier] NOT NULL,
	[parent_function_id] [uniqueidentifier] NULL,
	[app_id] [uniqueidentifier] NULL,
	[code] [varchar](50) NULL,
	[name] [varchar](50) NULL,
	[desc] [varchar](200) NULL,
	[ext_code_value] [varchar](4000) NULL,
	[is_inherit] [bit] NULL,
	[is_enable] [bit] NULL,
	[is_delete] [bit] NULL,
	[create_time] [datetime] NULL,
	[update_time] [datetime] NULL,
 CONSTRAINT [PK_auth_function_info] PRIMARY KEY CLUSTERED 
(
	[function_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[auth_group_info]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[auth_group_info](
	[group_id] [uniqueidentifier] NOT NULL,
	[name] [varchar](50) NULL,
	[create_time] [datetime] NULL,
 CONSTRAINT [PK_auth_group_info] PRIMARY KEY CLUSTERED 
(
	[group_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[auth_group_role]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[auth_group_role](
	[id] [uniqueidentifier] NOT NULL,
	[group_id] [uniqueidentifier] NULL,
	[role_id] [uniqueidentifier] NULL,
	[create_time] [datetime] NULL,
 CONSTRAINT [PK_auth_group_role] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[auth_role_function]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[auth_role_function](
	[id] [uniqueidentifier] NOT NULL,
	[role_id] [uniqueidentifier] NULL,
	[function_id] [uniqueidentifier] NULL,
	[create_time] [datetime] NULL,
 CONSTRAINT [PK_auth_role_function] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[auth_role_info]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[auth_role_info](
	[role_id] [uniqueidentifier] NOT NULL,
	[code] [varchar](50) NULL,
	[name] [varchar](50) NULL,
	[desc] [varchar](200) NULL,
	[is_enable] [bit] NULL,
	[is_delete] [bit] NULL,
	[create_time] [datetime] NULL,
	[update_time] [datetime] NULL,
 CONSTRAINT [PK_auth_role_info] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[auth_user_group]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[auth_user_group](
	[id] [uniqueidentifier] NOT NULL,
	[user_id] [uniqueidentifier] NULL,
	[group_id] [uniqueidentifier] NULL,
	[create_time] [datetime] NULL,
 CONSTRAINT [PK_auth_user_group] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[auth_user_info]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[auth_user_info](
	[user_id] [uniqueidentifier] NOT NULL,
	[ref_org_user_code] [varchar](50) NULL,
	[is_enable] [bit] NULL,
	[is_delete] [bit] NULL,
	[create_time] [datetime] NULL,
 CONSTRAINT [PK_auth_user_info] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[auth_user_role]    Script Date: 2016/7/29 17:30:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[auth_user_role](
	[id] [uniqueidentifier] NOT NULL,
	[user_id] [uniqueidentifier] NULL,
	[role_id] [uniqueidentifier] NULL,
	[create_time] [datetime] NULL,
 CONSTRAINT [PK_auth_user_role] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO