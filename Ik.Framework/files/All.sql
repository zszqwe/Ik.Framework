USE [Analysis]
GO
/****** Object:  Table [dbo].[auth_app_info]    Script Date: 2016/8/25 11:47:36 ******/
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
/****** Object:  Table [dbo].[auth_function_info]    Script Date: 2016/8/25 11:47:36 ******/
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
/****** Object:  Table [dbo].[auth_group_info]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[auth_group_info](
	[group_id] [uniqueidentifier] NOT NULL,
	[code] [varchar](50) NULL,
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
/****** Object:  Table [dbo].[auth_group_role]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[auth_group_role](
	[id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_auth_group_role_id]  DEFAULT (newid()),
	[group_id] [uniqueidentifier] NULL,
	[role_id] [uniqueidentifier] NULL,
	[create_time] [datetime] NULL CONSTRAINT [DF_auth_group_role_create_time]  DEFAULT (getdate()),
 CONSTRAINT [PK_auth_group_role] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[auth_role_function]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[auth_role_function](
	[id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_auth_role_function_id]  DEFAULT (newid()),
	[role_id] [uniqueidentifier] NULL,
	[function_id] [uniqueidentifier] NULL,
	[create_time] [datetime] NULL CONSTRAINT [DF_auth_role_function_create_time]  DEFAULT (getdate()),
 CONSTRAINT [PK_auth_role_function] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[auth_role_info]    Script Date: 2016/8/25 11:47:36 ******/
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
/****** Object:  Table [dbo].[auth_user_group]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[auth_user_group](
	[id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_auth_user_group_id]  DEFAULT (newid()),
	[user_id] [uniqueidentifier] NULL,
	[group_id] [uniqueidentifier] NULL,
	[create_time] [datetime] NULL CONSTRAINT [DF_auth_user_group_create_time]  DEFAULT (getdate()),
 CONSTRAINT [PK_auth_user_group] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[auth_user_info]    Script Date: 2016/8/25 11:47:36 ******/
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
	[desc] [varchar](200) NULL,
 CONSTRAINT [PK_auth_user_info] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[auth_user_role]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[auth_user_role](
	[id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_auth_user_role_id]  DEFAULT (newid()),
	[user_id] [uniqueidentifier] NULL,
	[role_id] [uniqueidentifier] NULL,
	[create_time] [datetime] NULL CONSTRAINT [DF_auth_user_role_create_time]  DEFAULT (getdate()),
 CONSTRAINT [PK_auth_user_role] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cache_ass_info]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cache_ass_info](
	[ass_id] [uniqueidentifier] NULL,
	[code] [varchar](50) NULL,
	[name] [varchar](50) NULL,
	[desc] [varchar](200) NULL,
	[create_time] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cache_key_app_info]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cache_key_app_info](
	[app_id] [uniqueidentifier] NULL,
	[code] [varchar](50) NULL,
	[name] [varchar](50) NULL,
	[desc] [varchar](200) NULL,
	[create_time] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cache_key_info]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cache_key_info](
	[key_id] [uniqueidentifier] NULL,
	[app_id] [uniqueidentifier] NULL,
	[code] [varchar](50) NULL,
	[name] [varchar](50) NULL,
	[key_scope] [int] NULL,
	[cache_type] [int] NULL,
	[ref_value_type] [varchar](50) NULL,
	[model_name] [varchar](50) NULL,
	[desc] [varchar](200) NULL,
	[create_time] [datetime] NULL,
	[update_time] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cache_value_types]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cache_value_types](
	[value_type_id] [uniqueidentifier] NULL,
	[ass_id] [uniqueidentifier] NULL,
	[name] [varchar](50) NULL,
	[code] [varchar](50) NULL,
	[class_context] [varchar](8000) NULL,
	[desc] [varchar](200) NULL,
	[create_time] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cfg_definition]    Script Date: 2016/8/25 11:47:36 ******/
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
/****** Object:  Table [dbo].[cfg_definition_item]    Script Date: 2016/8/25 11:47:36 ******/
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
/****** Object:  Table [dbo].[cfg_definition_version]    Script Date: 2016/8/25 11:47:36 ******/
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
/****** Object:  Table [dbo].[cfg_environment]    Script Date: 2016/8/25 11:47:36 ******/
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
/****** Object:  Table [dbo].[cfg_item]    Script Date: 2016/8/25 11:47:36 ******/
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
/****** Object:  Table [dbo].[cfg_version_environment_item]    Script Date: 2016/8/25 11:47:36 ******/
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
/****** Object:  Table [dbo].[cfg_version_item]    Script Date: 2016/8/25 11:47:36 ******/
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
/****** Object:  Table [dbo].[log_common]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[log_common](
	[log_id] [uniqueidentifier] NOT NULL,
	[app_name] [nvarchar](50) NOT NULL,
	[model_name] [nvarchar](50) NOT NULL,
	[business_name] [nvarchar](50) NOT NULL,
	[lable] [nvarchar](50) NOT NULL,
	[level] [smallint] NOT NULL,
	[code] [int] NOT NULL,
	[message] [nvarchar](150) NOT NULL,
	[create_time] [datetime] NOT NULL CONSTRAINT [DF_log_common_create_time]  DEFAULT (getdate()),
	[log_time] [datetime] NULL,
	[server_name] [nvarchar](30) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[log_common_content]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[log_common_content](
	[log_id] [uniqueidentifier] NOT NULL,
	[log_content] [nvarchar](max) NOT NULL,
	[create_time] [datetime] NOT NULL CONSTRAINT [DF_log_common_content_create_time]  DEFAULT (getdate())
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[log_request_api]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[log_request_api](
	[request_id] [uniqueidentifier] NULL,
	[application_name] [varchar](50) NULL,
	[server_name] [varchar](20) NULL,
	[client_ip] [varchar](20) NULL,
	[client_ip2] [varchar](20) NULL,
	[user_code] [varchar](50) NULL,
	[lng] [float] NULL,
	[lat] [float] NULL,
	[region_code] [int] NULL,
	[client_imei] [varchar](20) NULL,
	[client_net_type] [varchar](20) NULL,
	[interface_version] [varchar](20) NULL,
	[client_version] [varchar](20) NULL,
	[client_type] [varchar](20) NULL,
	[client_width] [int] NULL,
	[client_height] [int] NULL,
	[http_user_agent] [varchar](500) NULL,
	[http_method] [varchar](20) NULL,
	[http_status] [int] NULL,
	[request_time] [datetime] NULL,
	[request_elapsed_milliseconds] [int] NULL,
	[request_url] [varchar](500) NULL,
	[request_data] [varchar](4000) NULL,
	[request_headers] [varchar](500) NULL,
	[request_cookie] [varchar](500) NULL,
	[response_cookie] [varchar](500) NULL,
	[process_status] [int] NULL,
	[process_desc] [varchar](200) NULL,
	[create_time] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[log_request_api_result]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[log_request_api_result](
	[request_id] [uniqueidentifier] NULL,
	[result_content] [varchar](8000) NULL,
	[create_time] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[log_request_mvc]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[log_request_mvc](
	[request_id] [uniqueidentifier] NULL,
	[application_name] [varchar](50) NULL,
	[server_name] [varchar](50) NULL,
	[client_ip] [varchar](20) NULL,
	[client_ip2] [varchar](20) NULL,
	[user_code] [varchar](50) NULL,
	[client_width] [int] NULL,
	[client_height] [int] NULL,
	[http_user_agent] [varchar](500) NULL,
	[http_method] [varchar](20) NULL,
	[http_status] [int] NULL,
	[request_time] [datetime] NULL,
	[request_elapsed_milliseconds] [int] NULL,
	[request_url] [varchar](500) NULL,
	[request_data] [varchar](4000) NULL,
	[request_headers] [varchar](500) NULL,
	[request_cookie] [varchar](500) NULL,
	[response_cookie] [varchar](500) NULL,
	[create_time] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[log_request_mvc_result]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[log_request_mvc_result](
	[request_id] [uniqueidentifier] NULL,
	[result_content] [varchar](8000) NULL,
	[create_time] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[seq_number]    Script Date: 2016/8/25 11:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[seq_number](
	[seq_key] [varchar](50) NOT NULL,
	[name] [varchar](50) NULL,
	[next_value] [bigint] NULL,
	[prefix_value] [varchar](50) NULL,
	[format_length] [int] NULL,
	[date_format] [varchar](50) NULL,
	[apply_capacity] [int] NULL,
	[check_threshold] [int] NULL,
 CONSTRAINT [PK_seq_number] PRIMARY KEY CLUSTERED 
(
	[seq_key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'公共日志id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'log_common', @level2type=N'COLUMN',@level2name=N'log_id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'项目名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'log_common', @level2type=N'COLUMN',@level2name=N'app_name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'模块名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'log_common', @level2type=N'COLUMN',@level2name=N'model_name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'业务名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'log_common', @level2type=N'COLUMN',@level2name=N'business_name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'标签' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'log_common', @level2type=N'COLUMN',@level2name=N'lable'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志级别' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'log_common', @level2type=N'COLUMN',@level2name=N'level'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'log_common', @level2type=N'COLUMN',@level2name=N'code'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日志标题' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'log_common', @level2type=N'COLUMN',@level2name=N'message'
GO
