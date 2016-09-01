
GO
/****** Object:  Table [dbo].[log_common]    Script Date: 2016/8/10 13:59:41 ******/
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
/****** Object:  Table [dbo].[log_common_content]    Script Date: 2016/8/10 13:59:41 ******/
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
/****** Object:  Table [dbo].[log_request_api]    Script Date: 2016/8/10 13:59:41 ******/
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
	[http_user_agent] [varchar](100) NULL,
	[http_method] [varchar](20) NULL,
	[http_status] [int] NULL,
	[request_time] [datetime] NULL,
	[request_elapsed_milliseconds] [int] NULL,
	[request_url] [varchar](200) NULL,
	[request_data] [varchar](2000) NULL,
	[request_headers] [varchar](200) NULL,
	[request_cookie] [varchar](200) NULL,
	[response_cookie] [varchar](200) NULL,
	[process_status] [int] NULL,
	[process_desc] [varchar](200) NULL,
	[create_time] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[log_request_api_result]    Script Date: 2016/8/10 13:59:41 ******/
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
/****** Object:  Table [dbo].[log_request_mvc]    Script Date: 2016/8/10 13:59:41 ******/
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
	[http_user_agent] [varchar](100) NULL,
	[http_method] [varchar](20) NULL,
	[http_status] [int] NULL,
	[request_time] [datetime] NULL,
	[request_elapsed_milliseconds] [int] NULL,
	[request_url] [varchar](200) NULL,
	[request_data] [varchar](2000) NULL,
	[request_headers] [varchar](200) NULL,
	[request_cookie] [varchar](200) NULL,
	[response_cookie] [varchar](200) NULL,
	[create_time] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[log_request_mvc_result]    Script Date: 2016/8/10 13:59:41 ******/
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
