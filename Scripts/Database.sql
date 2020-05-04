CREATE DATABASE HELPAUTH;
USE HELPAUTH;

CREATE TABLE `userCredentials` (
	`username` varchar(50) NOT NULL,
	`password` varchar(32) NOT NULL,
	`role` varchar(10) NOT NULL,
	`login_blocked` timestamp DEFAULT NULL,
	`login_failed` int DEFAULT 0
	PRIMARY KEY (`username`)
);