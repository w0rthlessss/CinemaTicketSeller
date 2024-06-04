-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: cinematicketseller
-- ------------------------------------------------------
-- Server version	8.0.35

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `recipts`
--

DROP TABLE IF EXISTS `recipts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recipts` (
  `ReciptID` int NOT NULL,
  `CashierID` int NOT NULL,
  `Date` date DEFAULT NULL,
  `Time` tinytext CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci,
  `Tickets` json DEFAULT NULL,
  `Sum` int DEFAULT NULL,
  PRIMARY KEY (`ReciptID`),
  KEY `IX_Relationship15` (`CashierID`),
  CONSTRAINT `Relationship15` FOREIGN KEY (`CashierID`) REFERENCES `cashiers` (`CashierID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recipts`
--

LOCK TABLES `recipts` WRITE;
/*!40000 ALTER TABLE `recipts` DISABLE KEYS */;
INSERT INTO `recipts` VALUES (1,3,'2024-05-13','23:31:58','[{\"Price\": 250, \"SeatID\": 14, \"TicketID\": 1, \"ScreeningID\": 1}, {\"Price\": 250, \"SeatID\": 28, \"TicketID\": 2, \"ScreeningID\": 1}]',500),(2,3,'2024-05-24','14:28:20','[{\"Price\": 300, \"SeatID\": 154, \"TicketID\": 3, \"ScreeningID\": 44}, {\"Price\": 300, \"SeatID\": 153, \"TicketID\": 4, \"ScreeningID\": 44}]',600),(3,3,'2024-05-24','15:05:42','[{\"Price\": 250, \"SeatID\": 22, \"TicketID\": 5, \"ScreeningID\": 47}, {\"Price\": 250, \"SeatID\": 37, \"TicketID\": 6, \"ScreeningID\": 47}, {\"Price\": 250, \"SeatID\": 64, \"TicketID\": 7, \"ScreeningID\": 47}, {\"Price\": 250, \"SeatID\": 62, \"TicketID\": 8, \"ScreeningID\": 47}, {\"Price\": 250, \"SeatID\": 48, \"TicketID\": 9, \"ScreeningID\": 47}, {\"Price\": 300, \"SeatID\": 143, \"TicketID\": 10, \"ScreeningID\": 47}, {\"Price\": 300, \"SeatID\": 157, \"TicketID\": 11, \"ScreeningID\": 47}, {\"Price\": 300, \"SeatID\": 163, \"TicketID\": 12, \"ScreeningID\": 47}]',2150);
/*!40000 ALTER TABLE `recipts` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-06-04 16:02:04
