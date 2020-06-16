CREATE TABLE users
(
	`id` int NOT NULL,
	`firstname` varchar(20) NOT NULL,
	`lastname` varchar(20) NOT NULL,
	`username` varchar(20) NOT NULL,
	`password` varchar(32) NOT NULL,
	`role` varchar(7) NOT NULL,
	`login_blocked` timestamp DEFAULT NULL,
	`login_failed` int DEFAULT 0,
	`last_failed_attempt` timestamp DEFAULT NULL
);

CREATE TABLE patients
(
	`id` int PRIMARY KEY AUTO_INCREMENT,
	`firstname` varchar(20) NOT NULL,
	`lastname` varchar(20) NOT NULL,
	`gender` varchar(8) NOT NULL,
	`birthday` date NOT NULL,
	`nationality` varchar(100) DEFAULT NULL,
	`health_insurance` varchar(100) DEFAULT NULL,
	`kvnr` varchar(10) NOT NULL,
	`address` varchar(100) NOT NULL,
	`postalcode` varchar(5) NOT NULL,
	`city` varchar(60) NOT NULL,
	`phone` varchar(20) DEFAULT NULL,
	`mobile` varchar(20) DEFAULT NULL,
	`additional_informations` text DEFAULT NULL,
	`function_relatives` varchar(13) DEFAULT NULL,
	`firstname_relatives` varchar(20) DEFAULT NULL,
	`lastname_relatives` varchar(20) DEFAULT NULL,
	`address_relatives` varchar(100) DEFAULT NULL,
	`postalcode_relatives` varchar(5) DEFAULT NULL,
	`city_relatives` varchar(60) DEFAULT NULL,
	`phone_relatives` varchar(20) DEFAULT NULL,
	`mobile_relatives` varchar(20) DEFAULT NULL
);

CREATE TABLE cases
(
	`id` int PRIMARY KEY AUTO_INCREMENT,
	`patient_id` int NOT NULL,
	`priority` varchar(14) NOT NULL,
	`status` varchar(50) NOT NULL,
	`location` varchar(50) NOT NULL,
	`arrival` timestamp NOT NULL,
	`reevaluation` timestamp NOT NULL,
	`complaint` text NOT NULL,
	`type_of_arrival` text NOT NULL,
	`place_of_incident` text DEFAULT NULL,
	`trauma` text NOT NULL,
	`other_informations` text DEFAULT NULL,
	`anamnesis` text DEFAULT NULL,
	`services` text DEFAULT NULL,
	`external_services` text DEFAULT NULL,
	`physician_letter` text DEFAULT NULL,
	`diagnosis` text DEFAULT NULL,
	`type_of_release` varchar(36) DEFAULT NULL,
	`release` timestamp DEFAULT NULL,
	`medical_id` int DEFAULT NULL,
	`nurse_id` int DEFAULT NULL,
	`case_status` varchar(6) NOT NULL
);

CREATE TABLE vital_parameters
(
	`case_id` int NOT NULL,
	`time` timestamp NOT NULL,
	`heart_frequence` int DEFAULT NULL,
	`breath_frequence` int DEFAULT NULL,
	`bloodpressure_min` int DEFAULT NULL,
	`bloodpressure_max` int DEFAULT NULL,
	`temperature` double DEFAULT NULL,
	`oxygen_saturation` int DEFAULT NULL
);

CREATE TABLE locations
(
	`location` varchar(50) NOT NULL
);

CREATE TABLE statuses
(
	`status` varchar(50) NOT NULL
);

CREATE TABLE nationalities
(
	`nationality` varchar(100) NOT NULL
);

CREATE TABLE health_insurances
(
	`health_insurance` varchar(100) NOT NULL
);

CREATE TABLE priorities
(
	`priority` varchar(14) NOT NULL,
	`reevaluation_time` int NOT NULL
);