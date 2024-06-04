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
-- Table structure for table `screenings`
--

DROP TABLE IF EXISTS `screenings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `screenings` (
  `ScreeningID` int NOT NULL,
  `MovieID` int NOT NULL,
  `HallID` int DEFAULT NULL,
  `Date` date DEFAULT NULL,
  `Time` tinytext CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci,
  `PriceAmplification` float DEFAULT NULL,
  `OccupiedSeats` json DEFAULT NULL,
  PRIMARY KEY (`ScreeningID`),
  KEY `IX_Relationship7` (`MovieID`),
  CONSTRAINT `Relationship7` FOREIGN KEY (`MovieID`) REFERENCES `movies` (`MovieID`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `screenings`
--

LOCK TABLES `screenings` WRITE;
/*!40000 ALTER TABLE `screenings` DISABLE KEYS */;
INSERT INTO `screenings` VALUES (1,2,1,'2024-05-13','09:00:00',1,'[{\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 14, \"RowNumber\": 0, \"IsOccupied\": true, \"SeatNumber\": 13}, {\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 28, \"RowNumber\": 1, \"IsOccupied\": true, \"SeatNumber\": 13}]'),(2,7,3,'2024-05-13','12:00:00',1,NULL),(3,5,3,'2024-05-14','14:00:00',1,NULL),(4,2,3,'2024-05-01','16:00:00',1,NULL),(5,3,3,'2024-05-01','19:00:00',1,NULL),(6,5,2,'2024-05-01','22:00:00',1,NULL),(7,3,3,'2024-05-02','09:00:00',1,NULL),(8,7,1,'2024-05-02','12:00:00',1,NULL),(9,5,2,'2024-05-02','14:00:00',1,NULL),(10,6,1,'2024-05-02','16:00:00',1,NULL),(11,4,1,'2024-05-02','18:00:00',1,NULL),(12,1,3,'2024-05-02','20:00:00',1,NULL),(13,3,2,'2024-05-02','23:00:00',1,NULL),(14,1,2,'2024-05-03','09:00:00',1,NULL),(15,1,3,'2024-05-03','12:00:00',1,NULL),(16,5,2,'2024-05-03','15:00:00',1,NULL),(17,1,2,'2024-05-03','17:00:00',1,NULL),(18,6,3,'2024-05-03','20:00:00',1,NULL),(19,7,1,'2024-05-03','22:00:00',1,NULL),(23,3,3,'2024-05-04','17:00:00',1,NULL),(24,7,2,'2024-05-04','20:00:00',1,NULL),(25,4,2,'2024-05-12','22:00:00',1,NULL),(26,6,1,'2024-05-21','09:00:00',1,NULL),(27,1,1,'2024-05-21','11:00:00',1,NULL),(28,2,1,'2024-05-21','14:00:00',1,NULL),(29,3,1,'2024-05-21','17:00:00',1,NULL),(30,3,1,'2024-05-21','20:00:00',1,NULL),(31,6,1,'2024-05-21','23:00:00',1,NULL),(32,6,2,'2024-05-21','09:00:00',1,NULL),(33,1,2,'2024-05-21','11:00:00',1,NULL),(34,2,2,'2024-05-21','14:00:00',1,NULL),(35,3,2,'2024-05-21','17:00:00',1,NULL),(36,4,2,'2024-05-21','20:00:00',1,NULL),(37,3,2,'2024-05-21','22:00:00',1,NULL),(38,3,3,'2024-05-21','09:00:00',1,NULL),(39,7,3,'2024-05-21','12:00:00',1,NULL),(40,1,3,'2024-05-21','14:00:00',1,NULL),(41,1,3,'2024-05-21','17:00:00',1,NULL),(42,7,3,'2024-05-21','20:00:00',1,NULL),(43,7,3,'2024-05-21','22:00:00',1,NULL),(44,8,1,'2024-05-25','09:00:00',1,'[{\"Type\": \"couch\", \"Price\": 300, \"HallID\": 1, \"SeatID\": 154, \"RowNumber\": 10, \"IsOccupied\": true, \"SeatNumber\": 13}, {\"Type\": \"couch\", \"Price\": 300, \"HallID\": 1, \"SeatID\": 153, \"RowNumber\": 10, \"IsOccupied\": true, \"SeatNumber\": 12}]'),(45,5,1,'2024-05-25','11:00:00',1,NULL),(46,1,1,'2024-05-25','13:00:00',1,NULL),(47,4,1,'2024-05-25','16:00:00',1,'[{\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 22, \"RowNumber\": 1, \"IsOccupied\": true, \"SeatNumber\": 7}, {\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 37, \"RowNumber\": 2, \"IsOccupied\": true, \"SeatNumber\": 8}, {\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 64, \"RowNumber\": 4, \"IsOccupied\": true, \"SeatNumber\": 7}, {\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 62, \"RowNumber\": 4, \"IsOccupied\": true, \"SeatNumber\": 5}, {\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 48, \"RowNumber\": 3, \"IsOccupied\": true, \"SeatNumber\": 5}, {\"Type\": \"couch\", \"Price\": 300, \"HallID\": 1, \"SeatID\": 143, \"RowNumber\": 10, \"IsOccupied\": true, \"SeatNumber\": 2}, {\"Type\": \"couch\", \"Price\": 300, \"HallID\": 1, \"SeatID\": 157, \"RowNumber\": 11, \"IsOccupied\": true, \"SeatNumber\": 2}, {\"Type\": \"couch\", \"Price\": 300, \"HallID\": 1, \"SeatID\": 163, \"RowNumber\": 11, \"IsOccupied\": true, \"SeatNumber\": 8}]'),(48,3,1,'2024-05-25','18:00:00',1,NULL),(49,4,1,'2024-05-25','21:00:00',1,NULL),(50,6,1,'2024-05-25','23:00:00',1,NULL),(51,6,2,'2024-05-25','09:00:00',1,NULL),(52,5,2,'2024-05-25','11:00:00',1,NULL),(53,7,2,'2024-05-25','13:00:00',1,NULL),(54,7,2,'2024-05-25','15:00:00',1,NULL),(55,2,2,'2024-05-25','17:00:00',1,NULL),(56,7,2,'2024-05-25','20:00:00',1,NULL),(57,3,2,'2024-05-25','22:00:00',1,NULL),(58,5,3,'2024-05-25','09:00:00',1,NULL),(59,3,3,'2024-05-25','11:00:00',1,NULL),(60,6,3,'2024-05-25','14:00:00',1,NULL),(61,5,3,'2024-05-25','16:00:00',1,NULL),(62,1,3,'2024-05-25','18:00:00',1,NULL),(63,4,3,'2024-05-25','21:00:00',1,NULL),(64,3,3,'2024-05-25','23:00:00',1,NULL),(65,5,1,'2024-06-03','09:00:00',1,NULL),(66,4,1,'2024-06-03','11:00:00',1,NULL),(67,7,1,'2024-06-03','13:00:00',1,NULL),(68,4,1,'2024-06-03','15:00:00',1,NULL),(69,8,1,'2024-06-03','17:00:00',1,NULL),(70,8,1,'2024-06-03','19:00:00',1,NULL),(71,6,1,'2024-06-03','21:00:00',1,NULL),(72,1,1,'2024-06-03','23:00:00',1,NULL),(73,1,2,'2024-06-03','09:00:00',1,NULL),(74,5,2,'2024-06-03','12:00:00',1,NULL),(75,3,2,'2024-06-03','14:00:00',1,NULL),(76,2,2,'2024-06-03','17:00:00',1,NULL),(77,3,2,'2024-06-03','20:00:00',1,NULL),(78,7,2,'2024-06-03','23:00:00',1,NULL),(79,4,3,'2024-06-03','09:00:00',1,NULL),(80,7,3,'2024-06-03','11:00:00',1,NULL),(81,4,3,'2024-06-03','13:00:00',1,NULL),(82,7,3,'2024-06-03','15:00:00',1,NULL),(83,5,3,'2024-06-03','17:00:00',1,NULL),(84,7,3,'2024-06-03','19:00:00',1,NULL),(85,4,3,'2024-06-03','21:00:00',1,NULL),(86,5,3,'2024-06-03','23:00:00',1,NULL),(87,1,1,'2024-06-04','09:00:00',1,NULL),(88,4,1,'2024-06-04','12:00:00',1,NULL),(89,8,1,'2024-06-04','14:00:00',1,NULL),(90,7,1,'2024-06-04','16:00:00',1,NULL),(91,2,1,'2024-06-04','18:00:00',1,NULL),(92,8,1,'2024-06-04','21:00:00',1,NULL),(93,3,1,'2024-06-04','23:00:00',1,NULL),(94,6,2,'2024-06-04','09:00:00',1,NULL),(95,2,2,'2024-06-04','11:00:00',1,NULL),(96,1,2,'2024-06-04','14:00:00',1,NULL),(97,1,2,'2024-06-04','17:00:00',1,NULL),(98,8,2,'2024-06-04','20:00:00',1,NULL),(99,5,2,'2024-06-04','22:00:00',1,NULL),(100,2,3,'2024-06-04','09:00:00',1,NULL),(101,4,3,'2024-06-04','12:00:00',1,NULL),(102,8,3,'2024-06-04','14:00:00',1,NULL),(103,5,3,'2024-06-04','16:00:00',1,NULL),(104,8,3,'2024-06-04','18:00:00',1,NULL),(105,5,3,'2024-06-04','20:00:00',1,NULL),(106,6,3,'2024-06-04','22:00:00',1,NULL);
/*!40000 ALTER TABLE `screenings` ENABLE KEYS */;
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
