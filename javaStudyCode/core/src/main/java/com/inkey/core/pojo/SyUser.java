package com.inkey.core.pojo;

public class SyUser {
	/**
	 * 用户ID
	 */
	private int userId;

	/**
	 * 账号
	 */
	private String account;

	/**
	 * 密码 
	 */
	private String password;

	/**
	 * 用户真实姓名
	 */
	private String realName;
	
	/**
	 * 备注
	 */
	private String remark;
	
	

	public int getUserId() {
		return userId;
	}

	public void setUserId(int userId) {
		this.userId = userId;
	}

	public String getAccount() {
		return account;
	}

	public void setAccount(String account) {
		this.account = account;
	}

	public String getPassword() {
		return password;
	}

	public void setPassword(String password) {
		this.password = password;
	}

	public String getRealName() {
		return realName;
	}

	public void setRealName(String realName) {
		this.realName = realName;
	}

	public String getRemark() {
		return remark;
	}

	public void setRemark(String remark) {
		this.remark = remark;
	}
	
	
}
