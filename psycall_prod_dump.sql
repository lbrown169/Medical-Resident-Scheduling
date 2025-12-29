-- MySQL dump 10.13  Distrib 9.2.0, for macos15 (arm64)
--
-- Host: 127.0.0.1    Database: psycalldb
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__EFMigrationsHistory`
--

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES ('20250407044106_InitialCreate','6.0.3');
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `admins`
--

DROP TABLE IF EXISTS `admins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `admins` (
  `admin_id` varchar(15) NOT NULL,
  `first_name` varchar(45) NOT NULL,
  `last_name` varchar(45) NOT NULL,
  `email` varchar(45) NOT NULL,
  `password` varchar(100) NOT NULL,
  `phone_num` varchar(15) NOT NULL,
  PRIMARY KEY (`admin_id`),
  UNIQUE KEY `admin_id_UNIQUE` (`admin_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `admins`
--

LOCK TABLES `admins` WRITE;
/*!40000 ALTER TABLE `admins` DISABLE KEYS */;
INSERT INTO `admins` (`admin_id`, `first_name`, `last_name`, `email`, `password`, `phone_num`) VALUES ('0001','Tom','Hanks','tom.hanks@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0101'),('0002','Meryl','Streep','meryl.streep@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0102'),('0003','Denzel','Washington','denzel.washington@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0103'),('004','Scarlett','Johansson','scarlett.johansson@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0104');
/*!40000 ALTER TABLE `admins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `announcements`
--

DROP TABLE IF EXISTS `announcements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `announcements` (
  `announcement_id` binary(16) NOT NULL,
  `author_id` varchar(45) NOT NULL,
  `message` varchar(250) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`announcement_id`),
  UNIQUE KEY `announcement_id_UNIQUE` (`announcement_id`),
  KEY `author_id_idx` (`author_id`),
  CONSTRAINT `author_id` FOREIGN KEY (`author_id`) REFERENCES `admins` (`admin_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `announcements`
--

LOCK TABLES `announcements` WRITE;
/*!40000 ALTER TABLE `announcements` DISABLE KEYS */;
INSERT INTO `announcements` (`announcement_id`, `author_id`, `message`, `created_at`) VALUES (_binary '�[%K��','0002','Welcome to PsyCall!','2025-09-17 12:38:29');
/*!40000 ALTER TABLE `announcements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `blackouts`
--

DROP TABLE IF EXISTS `blackouts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `blackouts` (
  `blackout_id` binary(16) NOT NULL,
  `resident_id` varchar(15) NOT NULL,
  `date` date NOT NULL,
  PRIMARY KEY (`blackout_id`),
  UNIQUE KEY `blackout_id_UNIQUE` (`blackout_id`),
  KEY `resident_id_blackouts_idx` (`resident_id`),
  CONSTRAINT `resident_id_blackouts` FOREIGN KEY (`resident_id`) REFERENCES `residents` (`resident_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `blackouts`
--

LOCK TABLES `blackouts` WRITE;
/*!40000 ALTER TABLE `blackouts` DISABLE KEYS */;
/*!40000 ALTER TABLE `blackouts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dates`
--

DROP TABLE IF EXISTS `dates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dates` (
  `date_id` binary(16) NOT NULL,
  `schedule_id` binary(16) NOT NULL,
  `resident_id` varchar(15) NOT NULL,
  `date` date NOT NULL,
  `call_type` varchar(45) NOT NULL,
  `hours` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`date_id`),
  UNIQUE KEY `schedule_id_UNIQUE` (`date_id`),
  KEY `schedule_id` (`schedule_id`),
  KEY `resident_id_dates_idx` (`resident_id`),
  CONSTRAINT `resident_id_dates` FOREIGN KEY (`resident_id`) REFERENCES `residents` (`resident_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `schedule_id` FOREIGN KEY (`schedule_id`) REFERENCES `schedules` (`schedule_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dates`
--

LOCK TABLES `dates` WRITE;
/*!40000 ALTER TABLE `dates` DISABLE KEYS */;
/*!40000 ALTER TABLE `dates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `invitations`
--

DROP TABLE IF EXISTS `invitations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `invitations` (
  `token` varchar(255) NOT NULL,
  `resident_id` varchar(255) DEFAULT NULL,
  `expires` datetime DEFAULT NULL,
  `used` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`token`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invitations`
--

LOCK TABLES `invitations` WRITE;
/*!40000 ALTER TABLE `invitations` DISABLE KEYS */;
INSERT INTO `invitations` (`token`, `resident_id`, `expires`, `used`) VALUES ('11111111-1111-1111-1111-111111111111','BEA3374','2025-12-31 00:00:00',0),('22222222-2222-2222-2222-222222222222','COH3276','2025-12-15 00:00:00',1),('33333333-3333-3333-3333-333333333333','CTE3965','2025-12-10 00:00:00',0);
/*!40000 ALTER TABLE `invitations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `residents`
--

DROP TABLE IF EXISTS `residents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `residents` (
  `resident_id` varchar(15) NOT NULL,
  `first_name` varchar(45) NOT NULL,
  `last_name` varchar(45) NOT NULL,
  `graduate_yr` int NOT NULL DEFAULT '1',
  `email` varchar(45) NOT NULL,
  `password` varchar(100) NOT NULL DEFAULT '',
  `phone_num` varchar(15) NOT NULL DEFAULT '',
  `weekly_hours` int NOT NULL DEFAULT '0',
  `total_hours` int NOT NULL DEFAULT '0',
  `bi_yearly_hours` int NOT NULL DEFAULT '0',
  `hospital_role_profile` int DEFAULT NULL,
  PRIMARY KEY (`resident_id`),
  UNIQUE KEY `resident_id_UNIQUE` (`resident_id`),
  UNIQUE KEY `email_UNIQUE` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `residents`
--

LOCK TABLES `residents` WRITE;
/*!40000 ALTER TABLE `residents` DISABLE KEYS */;
INSERT INTO `residents` (`resident_id`, `first_name`, `last_name`, `graduate_yr`, `email`, `password`, `phone_num`, `weekly_hours`, `total_hours`, `bi_yearly_hours`, `hospital_role_profile`) VALUES ('BEA3374','Brad','Pitt',2,'brad.pitt@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0105',0,69,39,8),('COH3276','Angelina','Jolie',1,'angelina.jolie@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0106',0,27,27,0),('CTE3965','Leonardo','DiCaprio',2,'leonardo.dicaprio@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0107',0,57,21,13),('EIC4231','Natalie','Portman',3,'natalie.portman@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0108',0,9,9,NULL),('FEU3416','Robert','Downey',1,'robert.downey@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0109',0,21,9,5),('FVO3464','Chris','Evans',2,'chris.evans@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0110',0,63,45,9),('FXI2766','Chris','Hemsworth',1,'chris.hemsworth@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0111',0,24,24,2),('GEV4598','Emma','Stone',3,'emma.stone@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0112',0,9,9,NULL),('GKU3319','Ryan','Gosling',1,'ryan.gosling@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0113',0,33,9,6),('GMO4083','Jennifer','Lawrence',3,'jennifer.lawrence@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0114',0,9,9,NULL),('HKU2780','Morgan','Freeman',1,'morgan.freeman@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0115',0,30,9,4),('HQU5921','Cate','Blanchett',3,'cate.blanchett@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0116',0,9,9,NULL),('IDP3419','Joaquin','Phoenix',2,'joaquin.phoenix@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0117',0,75,21,11),('JCI5092','Samuel','Jackson',3,'samuel.jackson@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0118',0,9,9,NULL),('JXU4079','Keira','Knightley',2,'keira.knightley@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0119',0,54,21,12),('KOS3940','Hugh','Jackman',1,'hugh.jackman@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0120',0,39,39,3),('LLU6249','Christian','Bale',2,'christian.bale@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0121',0,78,18,10),('LZU4568','Anne','Hathaway',3,'anne.hathaway@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0122',0,9,9,NULL),('MGE3752','Will','Smith',1,'will.smith@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0123',0,45,9,7),('MPE3472','Zoe','Saldana',2,'zoe.saldana@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0124',0,60,21,15),('MPE3473','Matt','Damon',2,'matt.damon@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0125',0,51,27,14),('RCU4642','Amy','Adams',3,'amy.adams@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0126',0,9,9,NULL),('RRO4170','Mark','Ruffalo',3,'mark.ruffalo@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0127',0,9,9,NULL),('RUZ2717','Benedict','Cumberbatch',1,'benedict.cumberbatch@example.com','$2b$12$KqebvwNbxyuEg1OIOSjYZuMdXEDtEcbnsYwB9MHmSQgVsr.kHPWa.','201-555-0128',0,45,45,1);
/*!40000 ALTER TABLE `residents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rotations`
--

DROP TABLE IF EXISTS `rotations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rotations` (
  `rotation_id` binary(16) NOT NULL,
  `resident_id` varchar(15) NOT NULL,
  `month` varchar(45) NOT NULL,
  `rotation` varchar(45) NOT NULL,
  PRIMARY KEY (`rotation_id`),
  UNIQUE KEY `rotation_id_UNIQUE` (`rotation_id`),
  KEY `resident_id_rotation_idx` (`resident_id`),
  CONSTRAINT `resident_id_rotation` FOREIGN KEY (`resident_id`) REFERENCES `residents` (`resident_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rotations`
--

LOCK TABLES `rotations` WRITE;
/*!40000 ALTER TABLE `rotations` DISABLE KEYS */;
INSERT INTO `rotations` (`rotation_id`, `resident_id`, `month`, `rotation`) VALUES (_binary '�f��R@\�','FXI2766','June','IMOP'),(_binary '�.�� K6\�','LLU6249','February','ER P/CL'),(_binary '���^M\�','MPE3473','February','Addiction'),(_binary '�3q[I]�\�','MPE3472','August','ER P/CL'),(_binary '��Q�A9\�','CTE3965','September','Inpt Psy'),(_binary '���N�','HKU2780','January','Inpt Psy'),(_binary '�2��G$�','GKU3319','March','ER-P'),(_binary '\r\"qf�G��\�','HKU2780','August','Neuro'),(_binary '-�oe�F�\"\r','MGE3752','September','Neuro'),(_binary '`O�,KC��\�','HKU2780','December','Neuro'),(_binary '���]�K','JXU4079','February','Child'),(_binary '�Q�P@�\�','MPE3473','January','Comm'),(_binary '��/�J�','MGE3752','November','EM'),(_binary '��tW	J`�\�','RUZ2717','September','ER-P'),(_binary '����9Bz','GKU3319','August','EM'),(_binary '�0�[+@U�\�','FVO3464','July','Comm'),(_binary 'v�KIt�Z%\�','FVO3464','December','Child'),(_binary '��xK馣\'\�','LLU6249','November','Float'),(_binary '�~�h\nF�','JXU4079','April','Float'),(_binary '4��OJ8�\�','FVO3464','June','PHP/IOP'),(_binary '��`�{I࣌','MPE3472','January','Comm'),(_binary 'X��l�K�','LLU6249','June','Inpt Psy'),(_binary 'Ɨ\\AB��','KOS3940','January','EM'),(_binary 'u4��tO]�j','FEU3416','March','EM'),(_binary ' ���*H�','CTE3965','March','Child'),(_binary '\",�`Y�G�`','CTE3965','May','Float'),(_binary '\"f�9%�Ah�\�','HKU2780','October','IMOP'),(_binary '%gJ\']EK��H','JXU4079','September','PHP/IOP'),(_binary '%C\0��HNݽ�','BEA3374','January','Geri'),(_binary '%�`�O�\�','FEU3416','January','IMOP'),(_binary '&�00.Nj��','KOS3940','June','Neuro'),(_binary '&�t�J\0FT�\�','HKU2780','March','Inpt Psy'),(_binary '\'��PK�\�','GKU3319','November','Neuro'),(_binary ')�ͩqL֚�kL','CTE3965','January','Addiction'),(_binary ',n���Ef\�','FEU3416','July','Consult'),(_binary ',>-ǬCϻ\n��','BEA3374','April','Inpt Psy'),(_binary '-\"�	�sO/�\�','MPE3472','November','Geri'),(_binary '/7�NC��\�','CTE3965','July','Geri'),(_binary '/��;]xK�\�','FEU3416','May','IMIP'),(_binary '/��t̷L�\�','BEA3374','May','PHP/IOP'),(_binary '0Wz�	NI��\�','FVO3464','March','Inpt Psy'),(_binary '1�.C�F<�\�','BEA3374','December','Float'),(_binary '1_bg��D�_','FXI2766','May','EM'),(_binary '2pa�xB��3','FXI2766','September','Consult'),(_binary '3\\r�S�B�\�','KOS3940','March','Neuro'),(_binary '4%P�\ZI�I\�','MPE3472','September','Consult'),(_binary '4ISO��NU�\�','RUZ2717','January','Neuro'),(_binary '5�et��Aȸ','FEU3416','October','Inpt Psy'),(_binary '9��y��J1','COH3276','August','IMOP'),(_binary '9�����','GKU3319','June','Inpt Psy'),(_binary '<�0�©`E�\�','JXU4079','May','Forensic'),(_binary '<�����','COH3276','March','Inpt Psy'),(_binary '?���+K/�','CTE3965','April','Comm'),(_binary '@�ֱ�G�\�','LLU6249','April','Consult'),(_binary 'A{5��M=�\�','RUZ2717','December','Inpt Psy'),(_binary 'A�5�(�Cu\�','BEA3374','March','Inpt Psy'),(_binary 'A�;�@�\�','IDP3419','July','Addiction'),(_binary 'BZ̚��J4�\Z','FXI2766','January','Neuro'),(_binary 'B��\'��M','MPE3473','April','Child'),(_binary 'E0�;��Ah\�','CTE3965','August','CL/ER P'),(_binary 'F����Mx','FEU3416','April','IMOP'),(_binary 'F����@\�','RUZ2717','October','Inpt Psy'),(_binary 'G�U��fI�','RUZ2717','April','IMOP'),(_binary 'HōQwJ��D\�','FEU3416','December','ER-P'),(_binary 'I�L5IL��\�','IDP3419','June','Geri'),(_binary 'J���E�\�','IDP3419','January','CL/ER P'),(_binary 'K�gO��K�','KOS3940','August','Inpt Psy'),(_binary 'N���}A�','HKU2780','September','IMIP'),(_binary 'P����K','JXU4079','November','Inpt Psy'),(_binary 'R�(1MG�F\�','COH3276','May','Inpt Psy'),(_binary 'SZnDA��$\�','COH3276','June','Inpt Psy'),(_binary 'S�*)��@J\�','MGE3752','March','Consult'),(_binary 'S�p�PE�\�','FVO3464','August','Forensic'),(_binary 'Uag��B�\�','HKU2780','April','Consult'),(_binary 'X�3�Ct�\�','RUZ2717','May','Neuro'),(_binary '[%��F�\�','GKU3319','January','Consult'),(_binary '[|S���E�','MGE3752','July','IMOP'),(_binary '\\��%K�\�','FEU3416','September','Inpt Psy'),(_binary '`{�~��Eԛ','MGE3752','December','IMOP'),(_binary '`ˉՅ�@�M\r','MPE3473','September','Consult'),(_binary '`�-�.�@�','LLU6249','September','Comm'),(_binary 'b(C饯BM��\�','KOS3940','July','Inpt Psy'),(_binary 'c*��v�H�','BEA3374','November','Comm'),(_binary 'c�Y�I��','KOS3940','December','Inpt Psy'),(_binary 'drеh��G�\�','MPE3473','August','ER P/CL'),(_binary 'hQ?60F��綗','IDP3419','November','Forensic'),(_binary 'h���V@\�','JXU4079','March','Addiction'),(_binary 'j�w��Ok�','FVO3464','April','Inpt Psy'),(_binary 'k!N�0Ew�Z\�','BEA3374','June','Consult'),(_binary 'mt��+Iڣ�','MPE3472','October','PHP/IOP'),(_binary 'nz��j?Dĥ�','MPE3472','July','Inpt Psy'),(_binary 'of6p��H�\�','FEU3416','February','Neuro'),(_binary 'pd\\;-Nw��gW','MPE3472','February','Addiction'),(_binary 'q7J�+Mȫ��','MGE3752','June','Inpt Psy'),(_binary 'qc��Ȫ�E\�','JXU4079','December','Consult'),(_binary 's�s�K\'C�Ae','MPE3472','April','Child'),(_binary 'u�9}-KT�c�','MGE3752','August','IMIP'),(_binary 'wW�fnW@t��','LLU6249','January','Inpt Psy'),(_binary 'zΘosDM~�:�','GKU3319','May','Inpt Psy'),(_binary '{d(�ͶE��','JXU4079','August','Geri'),(_binary '|9*6I�F��\�','COH3276','December','Neuro'),(_binary '��&�vN1\�','LLU6249','August','Child'),(_binary 'ı!��Kˁ3\�','IDP3419','August','Child'),(_binary 'ｏ��&G�\�','HKU2780','July','EM'),(_binary '�\0ٵQ�G?�\�','IDP3419','October','Float'),(_binary '�:S��G%\�','MPE3472','December','Inpt Psy'),(_binary '�RVUN8��','RUZ2717','February','IMOP'),(_binary '��N�Ch\�','COH3276','July','Neuro'),(_binary '�\n���ZF\�','MGE3752','January','Inpt Psy'),(_binary '���9Je�','MGE3752','February','Inpt Psy'),(_binary '�A�`@!�2','RUZ2717','November','Inpt Psy'),(_binary '��9��H','JXU4079','July','ER P/CL'),(_binary '�b~^�#G�i','JXU4079','June','Comm'),(_binary '�0\r�@C�l','CTE3965','October','Inpt Psy'),(_binary '��.\"@�\�','IDP3419','February','Inpt Psy'),(_binary '����L\�','CTE3965','June','Forensic'),(_binary '�gDlFI�^/\�','MPE3473','October','PHP/IOP'),(_binary '�*�lPD�\�','MGE3752','May','ER-P'),(_binary '�AZ�@ݼ�','KOS3940','May','IMOP'),(_binary '�+.y7BǙ{&M˝','GKU3319','April','Inpt Psy'),(_binary '�,=��K�\�','IDP3419','September','Child'),(_binary '�-ԉ��I�','FVO3464','November','Child'),(_binary '�/ý)�@�\�','FVO3464','October','Addiction'),(_binary '�2u|�CҒ:�','CTE3965','February','Child'),(_binary '�2w�^�B�','RUZ2717','July','Inpt Psy'),(_binary '�41~c�H�\�','KOS3940','February','IMOP'),(_binary '�4��.�L\�','MPE3472','June','Float'),(_binary '�5����','HKU2780','November','IMOP'),(_binary '�:-HN��','COH3276','September','EM'),(_binary '�?^�@�\�','MPE3473','July','Inpt Psy'),(_binary '�@�={F�\�','HKU2780','May','Inpt Psy'),(_binary '�A˒��M�','COH3276','January','Inpt Psy'),(_binary '�F)��LM\�','GKU3319','December','IMOP'),(_binary '�Fᎈ:N.��','FXI2766','October','ER-P'),(_binary '�Gq;�D��','GKU3319','February','Inpt Psy'),(_binary '�J��@�','BEA3374','February','CL/ER P'),(_binary '�J����','GKU3319','July','Neuro'),(_binary '�M�jZ�N�','BEA3374','October','Child'),(_binary '�N=	\n-�L�\�','MPE3473','November','Geri'),(_binary '�TILD��\�','COH3276','October','IMOP'),(_binary '�U�2�%F5\�','FXI2766','July','Inpt Psy'),(_binary '�V���vCl','BEA3374','July','Forensic'),(_binary '�V����','FVO3464','February','Geri'),(_binary '�W�*�SB\�','GKU3319','October','IMIP'),(_binary '�Yٜ�O� p','COH3276','April','ER-P'),(_binary '�^Uw��Hʜ\�','RUZ2717','March','IMIP'),(_binary '�__5�\\LT�r','HKU2780','June','ER-P'),(_binary '�_�io@�b\�','JXU4079','January','Child'),(_binary '�fK��J�','BEA3374','August','Addiction'),(_binary '�l�Q2�C+\�','HKU2780','February','Inpt Psy'),(_binary '�l���;H\\','LLU6249','July','Child'),(_binary '�qǔ�7U@�\�','MPE3473','December','Inpt Psy'),(_binary '�rd��`�O','MPE3473','May','Child'),(_binary '�t�2�J�','IDP3419','March','Consult'),(_binary '�v���gH\�','FXI2766','November','Inpt Psy'),(_binary '�x�Q��O','FVO3464','September','Float'),(_binary '�z}ߒ�I�\�','COH3276','November','IMIP'),(_binary '�|���C\�','MPE3472','March','Forensic'),(_binary '�}�\r]�Ix\�','FVO3464','January','ER P/CL'),(_binary '�ō�:L�\�','IDP3419','December','Comm'),(_binary '��	�(A\�','MPE3473','June','Float'),(_binary '���R\Z�J','MPE3473','March','Forensic'),(_binary '����C\�','LLU6249','May','Geri'),(_binary '��i�D8\�','LLU6249','December','Addiction'),(_binary '�����','FEU3416','November','Inpt Psy'),(_binary '��Q\Z�Ou\�','CTE3965','December','PHP/IOP'),(_binary '��\"�lLQ\�','MGE3752','October','Neuro'),(_binary '��\'�4EL�','KOS3940','October','Consult'),(_binary '��(�=F{\�','RUZ2717','August','Consult'),(_binary '��6�I�','KOS3940','November','ER-P'),(_binary '��78#E�\�','CTE3965','November','Consult'),(_binary '��<�(�A\�','FVO3464','May','Consult'),(_binary '��=mIG�','IDP3419','April','PHP/IOP'),(_binary '��P���','FEU3416','August','Inpt Psy'),(_binary '��R�T�Ev','MPE3472','May','Child'),(_binary '��^��N\�','FXI2766','December','Inpt Psy'),(_binary '��f��Do','FXI2766','February','IMIP'),(_binary '��q��D','COH3276','February','Consult'),(_binary '��tAs�\�','IDP3419','May','Inpt Psy'),(_binary '��u5ruL�\�','KOS3940','April','IMIP'),(_binary '��z��sE\�','FEU3416','June','Neuro'),(_binary '��؇TH�\�','MGE3752','April','Inpt Psy'),(_binary '��ޫc�@\�','JXU4079','October','Inpt Psy'),(_binary '���2zNj�','FXI2766','April','Neuro'),(_binary '���X��','FXI2766','August','Inpt Psy'),(_binary '���[q�@\�','LLU6249','October','Forensic'),(_binary '���t�L\�','RUZ2717','June','EM'),(_binary '����5�','GKU3319','September','IMOP'),(_binary '����}�','FXI2766','March','IMOP'),(_binary '�����','BEA3374','September','Child'),(_binary '�����J','LLU6249','March','PHP/IOP'),(_binary '�����\�','KOS3940','September','Inpt Psy');
/*!40000 ALTER TABLE `rotations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `schedules`
--

DROP TABLE IF EXISTS `schedules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `schedules` (
  `schedule_id` binary(16) NOT NULL,
  `generated_year` int NOT NULL,
  `status` varchar(45) NOT NULL,
  PRIMARY KEY (`schedule_id`),
  UNIQUE KEY `schedule_id_UNIQUE` (`schedule_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `schedules`
--

LOCK TABLES `schedules` WRITE;
/*!40000 ALTER TABLE `schedules` DISABLE KEYS */;
/*!40000 ALTER TABLE `schedules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `swap_requests`
--

DROP TABLE IF EXISTS `swap_requests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `swap_requests` (
  `idswap_requests` binary(16) NOT NULL,
  `schedule_swap_id` binary(16) NOT NULL,
  `requester_id` varchar(15) NOT NULL,
  `requestee_id` varchar(15) NOT NULL,
  `requester_date` date NOT NULL,
  `requestee_date` date NOT NULL,
  `status` varchar(45) NOT NULL DEFAULT 'Pending',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `details` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`idswap_requests`),
  UNIQUE KEY `idswap_requests_UNIQUE` (`idswap_requests`),
  KEY `requester_id_idx` (`requester_id`),
  KEY `requestee_id_idx` (`requestee_id`),
  KEY `schedule_id_idx` (`schedule_swap_id`),
  CONSTRAINT `requestee_id` FOREIGN KEY (`requestee_id`) REFERENCES `residents` (`resident_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `requester_id` FOREIGN KEY (`requester_id`) REFERENCES `residents` (`resident_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `schedule_swap_id` FOREIGN KEY (`schedule_swap_id`) REFERENCES `schedules` (`schedule_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `swap_requests`
--

LOCK TABLES `swap_requests` WRITE;
/*!40000 ALTER TABLE `swap_requests` DISABLE KEYS */;
/*!40000 ALTER TABLE `swap_requests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vacations`
--

DROP TABLE IF EXISTS `vacations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vacations` (
  `vacation_id` binary(16) NOT NULL,
  `resident_id` varchar(15) NOT NULL,
  `date` date NOT NULL,
  `reason` varchar(45) DEFAULT NULL,
  `status` varchar(45) NOT NULL DEFAULT 'Pending',
  `details` varchar(150) DEFAULT NULL,
  `groupId` varchar(36) NOT NULL,
  PRIMARY KEY (`vacation_id`),
  KEY `idx_resident_id` (`resident_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vacations`
--

LOCK TABLES `vacations` WRITE;
/*!40000 ALTER TABLE `vacations` DISABLE KEYS */;
/*!40000 ALTER TABLE `vacations` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-12-15 21:48:05
