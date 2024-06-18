CREATE DATABASE  IF NOT EXISTS `cinematicketseller` /*!40100 DEFAULT CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `cinematicketseller`;
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
INSERT INTO `screenings` VALUES (1,2,1,'2024-05-13','09:00:00',1,'[{\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 14, \"RowNumber\": 0, \"IsOccupied\": true, \"SeatNumber\": 13}, {\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 28, \"RowNumber\": 1, \"IsOccupied\": true, \"SeatNumber\": 13}]'),(2,7,3,'2024-05-13','12:00:00',1,NULL),(3,5,3,'2024-05-14','14:00:00',1,NULL),(4,2,3,'2024-05-01','16:00:00',1,NULL),(5,3,3,'2024-05-01','19:00:00',1,NULL),(6,5,2,'2024-05-01','22:00:00',1,NULL),(7,3,3,'2024-05-02','09:00:00',1,NULL),(8,7,1,'2024-05-02','12:00:00',1,NULL),(9,5,2,'2024-05-02','14:00:00',1,NULL),(10,6,1,'2024-05-02','16:00:00',1,NULL),(11,4,1,'2024-05-02','18:00:00',1,NULL),(12,1,3,'2024-05-02','20:00:00',1,NULL),(13,3,2,'2024-05-02','23:00:00',1,NULL),(14,1,2,'2024-05-03','09:00:00',1,NULL),(15,1,3,'2024-05-03','12:00:00',1,NULL),(16,5,2,'2024-05-03','15:00:00',1,NULL),(17,1,2,'2024-05-03','17:00:00',1,NULL),(18,6,3,'2024-05-03','20:00:00',1,NULL),(19,7,1,'2024-05-03','22:00:00',1,NULL),(23,3,3,'2024-05-04','17:00:00',1,NULL),(24,7,2,'2024-05-04','20:00:00',1,NULL),(25,4,2,'2024-05-12','22:00:00',1,NULL),(26,6,1,'2024-05-21','09:00:00',1,NULL),(27,1,1,'2024-05-21','11:00:00',1,NULL),(28,2,1,'2024-05-21','14:00:00',1,NULL),(29,3,1,'2024-05-21','17:00:00',1,NULL),(30,3,1,'2024-05-21','20:00:00',1,NULL),(31,6,1,'2024-05-21','23:00:00',1,NULL),(32,6,2,'2024-05-21','09:00:00',1,NULL),(33,1,2,'2024-05-21','11:00:00',1,NULL),(34,2,2,'2024-05-21','14:00:00',1,NULL),(35,3,2,'2024-05-21','17:00:00',1,NULL),(36,4,2,'2024-05-21','20:00:00',1,NULL),(37,3,2,'2024-05-21','22:00:00',1,NULL),(38,3,3,'2024-05-21','09:00:00',1,NULL),(39,7,3,'2024-05-21','12:00:00',1,NULL),(40,1,3,'2024-05-21','14:00:00',1,NULL),(41,1,3,'2024-05-21','17:00:00',1,NULL),(42,7,3,'2024-05-21','20:00:00',1,NULL),(43,7,3,'2024-05-21','22:00:00',1,NULL),(44,8,1,'2024-05-25','09:00:00',1,'[{\"Type\": \"couch\", \"Price\": 300, \"HallID\": 1, \"SeatID\": 154, \"RowNumber\": 10, \"IsOccupied\": true, \"SeatNumber\": 13}, {\"Type\": \"couch\", \"Price\": 300, \"HallID\": 1, \"SeatID\": 153, \"RowNumber\": 10, \"IsOccupied\": true, \"SeatNumber\": 12}]'),(45,5,1,'2024-05-25','11:00:00',1,NULL),(46,1,1,'2024-05-25','13:00:00',1,NULL),(47,4,1,'2024-05-25','16:00:00',1,'[{\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 22, \"RowNumber\": 1, \"IsOccupied\": true, \"SeatNumber\": 7}, {\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 37, \"RowNumber\": 2, \"IsOccupied\": true, \"SeatNumber\": 8}, {\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 64, \"RowNumber\": 4, \"IsOccupied\": true, \"SeatNumber\": 7}, {\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 62, \"RowNumber\": 4, \"IsOccupied\": true, \"SeatNumber\": 5}, {\"Type\": \"regular\", \"Price\": 250, \"HallID\": 1, \"SeatID\": 48, \"RowNumber\": 3, \"IsOccupied\": true, \"SeatNumber\": 5}, {\"Type\": \"couch\", \"Price\": 300, \"HallID\": 1, \"SeatID\": 143, \"RowNumber\": 10, \"IsOccupied\": true, \"SeatNumber\": 2}, {\"Type\": \"couch\", \"Price\": 300, \"HallID\": 1, \"SeatID\": 157, \"RowNumber\": 11, \"IsOccupied\": true, \"SeatNumber\": 2}, {\"Type\": \"couch\", \"Price\": 300, \"HallID\": 1, \"SeatID\": 163, \"RowNumber\": 11, \"IsOccupied\": true, \"SeatNumber\": 8}]'),(48,3,1,'2024-05-25','18:00:00',1,NULL),(49,4,1,'2024-05-25','21:00:00',1,NULL),(50,6,1,'2024-05-25','23:00:00',1,NULL),(51,6,2,'2024-05-25','09:00:00',1,NULL),(52,5,2,'2024-05-25','11:00:00',1,NULL),(53,7,2,'2024-05-25','13:00:00',1,NULL),(54,7,2,'2024-05-25','15:00:00',1,NULL),(55,2,2,'2024-05-25','17:00:00',1,NULL),(56,7,2,'2024-05-25','20:00:00',1,NULL),(57,3,2,'2024-05-25','22:00:00',1,NULL),(58,5,3,'2024-05-25','09:00:00',1,NULL),(59,3,3,'2024-05-25','11:00:00',1,NULL),(60,6,3,'2024-05-25','14:00:00',1,NULL),(61,5,3,'2024-05-25','16:00:00',1,NULL),(62,1,3,'2024-05-25','18:00:00',1,NULL),(63,4,3,'2024-05-25','21:00:00',1,NULL),(64,3,3,'2024-05-25','23:00:00',1,NULL),(65,5,1,'2024-06-03','09:00:00',1,NULL),(66,4,1,'2024-06-03','11:00:00',1,NULL),(67,7,1,'2024-06-03','13:00:00',1,NULL),(68,4,1,'2024-06-03','15:00:00',1,NULL),(69,8,1,'2024-06-03','17:00:00',1,NULL),(70,8,1,'2024-06-03','19:00:00',1,NULL),(71,6,1,'2024-06-03','21:00:00',1,NULL),(72,1,1,'2024-06-03','23:00:00',1,NULL),(73,1,2,'2024-06-03','09:00:00',1,NULL),(74,5,2,'2024-06-03','12:00:00',1,NULL),(75,3,2,'2024-06-03','14:00:00',1,NULL),(76,2,2,'2024-06-03','17:00:00',1,NULL),(77,3,2,'2024-06-03','20:00:00',1,NULL),(78,7,2,'2024-06-03','23:00:00',1,NULL),(79,4,3,'2024-06-03','09:00:00',1,NULL),(80,7,3,'2024-06-03','11:00:00',1,NULL),(81,4,3,'2024-06-03','13:00:00',1,NULL),(82,7,3,'2024-06-03','15:00:00',1,NULL),(83,5,3,'2024-06-03','17:00:00',1,NULL),(84,7,3,'2024-06-03','19:00:00',1,NULL),(85,4,3,'2024-06-03','21:00:00',1,NULL),(86,5,3,'2024-06-03','23:00:00',1,NULL),(87,2,1,'2024-06-04','09:00:00',1,NULL),(88,5,1,'2024-06-04','12:00:00',1,NULL),(89,5,1,'2024-06-04','14:00:00',1,NULL),(90,7,1,'2024-06-04','16:00:00',1,NULL),(91,3,1,'2024-06-04','18:00:00',1,NULL),(92,5,1,'2024-06-04','21:00:00',1,NULL),(93,8,1,'2024-06-04','23:00:00',1,NULL),(94,5,2,'2024-06-04','09:00:00',1,NULL),(95,2,2,'2024-06-04','11:00:00',1,NULL),(96,3,2,'2024-06-04','14:00:00',1,NULL),(97,6,2,'2024-06-04','17:00:00',1,NULL),(98,1,2,'2024-06-04','19:00:00',1,NULL),(99,3,2,'2024-06-04','22:00:00',1,NULL),(100,6,3,'2024-06-04','09:00:00',1,NULL),(101,6,3,'2024-06-04','11:00:00',1,NULL),(102,7,3,'2024-06-04','13:00:00',1,NULL),(103,2,3,'2024-06-04','15:00:00',1,NULL),(104,6,3,'2024-06-04','18:00:00',1,NULL),(105,6,3,'2024-06-04','20:00:00',1,NULL),(106,7,3,'2024-06-04','22:00:00',1,NULL),(107,4,1,'2024-06-05','09:00:00',1,NULL),(108,3,1,'2024-06-05','11:00:00',1,NULL),(109,7,1,'2024-06-05','14:00:00',1,NULL),(110,4,1,'2024-06-05','16:00:00',1,NULL),(111,1,1,'2024-06-05','18:00:00',1,NULL),(112,1,1,'2024-06-05','21:00:00',1,NULL),(113,2,2,'2024-06-05','09:00:00',1,NULL),(114,5,2,'2024-06-05','12:00:00',1,NULL),(115,4,2,'2024-06-05','14:00:00',1,NULL),(116,6,2,'2024-06-05','16:00:00',1,NULL),(117,2,2,'2024-06-05','18:00:00',1,NULL),(118,3,2,'2024-06-05','21:00:00',1,NULL),(119,1,3,'2024-06-05','09:00:00',1,NULL),(120,8,3,'2024-06-05','12:00:00',1,NULL),(121,6,3,'2024-06-05','14:00:00',1,NULL),(122,5,3,'2024-06-05','16:00:00',1,NULL),(123,6,3,'2024-06-05','18:00:00',1,NULL),(124,2,3,'2024-06-05','20:00:00',1,NULL),(125,4,3,'2024-06-05','23:00:00',1.5,NULL),(126,1,1,'2024-06-06','09:00:00',1,NULL),(127,2,1,'2024-06-06','12:00:00',1,NULL),(128,7,1,'2024-06-06','15:00:00',1,NULL),(129,8,1,'2024-06-06','17:00:00',1,NULL),(130,1,1,'2024-06-06','19:00:00',1,NULL),(131,5,1,'2024-06-06','22:00:00',1,NULL),(132,6,2,'2024-06-06','09:00:00',1,NULL),(133,1,2,'2024-06-06','11:00:00',1,NULL),(134,2,2,'2024-06-06','14:00:00',1,NULL),(135,2,2,'2024-06-06','17:00:00',1,NULL),(136,4,2,'2024-06-06','20:00:00',1,NULL),(137,3,2,'2024-06-06','22:00:00',1,NULL),(138,4,3,'2024-06-06','09:00:00',1,NULL),(139,1,3,'2024-06-06','11:00:00',1,NULL),(140,8,3,'2024-06-06','14:00:00',1,NULL),(141,2,3,'2024-06-06','16:00:00',1,NULL),(142,7,3,'2024-06-06','19:00:00',1,NULL),(143,5,3,'2024-06-06','21:00:00',1,NULL),(144,2,3,'2024-06-06','23:00:00',1,NULL),(145,5,1,'2024-06-07','09:00:00',1,NULL),(146,1,1,'2024-06-07','11:00:00',1,NULL),(147,6,1,'2024-06-07','14:00:00',1,NULL),(148,2,1,'2024-06-07','16:00:00',1,NULL),(149,5,1,'2024-06-07','19:00:00',1,NULL),(150,5,1,'2024-06-07','21:00:00',1,NULL),(151,1,1,'2024-06-07','23:00:00',1,NULL),(152,6,2,'2024-06-07','09:00:00',1,NULL),(153,5,2,'2024-06-07','11:00:00',1,NULL),(154,6,2,'2024-06-07','13:00:00',1,NULL),(155,8,2,'2024-06-07','15:00:00',1,NULL),(156,4,2,'2024-06-07','17:00:00',1,NULL),(157,1,2,'2024-06-07','19:00:00',1,NULL),(158,3,2,'2024-06-07','22:00:00',1,NULL),(159,1,3,'2024-06-07','09:00:00',1,NULL),(160,2,3,'2024-06-07','12:00:00',1,NULL),(161,3,3,'2024-06-07','15:00:00',1,NULL),(162,1,3,'2024-06-07','18:00:00',1,NULL),(163,1,3,'2024-06-07','21:00:00',1,NULL),(164,8,1,'2024-06-08','09:00:00',1,NULL),(165,1,1,'2024-06-08','11:00:00',1,NULL),(166,2,1,'2024-06-08','14:00:00',1,NULL),(167,3,1,'2024-06-08','17:00:00',1,NULL),(168,1,1,'2024-06-08','20:00:00',1,NULL),(169,5,1,'2024-06-08','23:00:00',1,NULL),(170,4,2,'2024-06-08','09:00:00',1,NULL),(171,7,2,'2024-06-08','11:00:00',1,NULL),(172,5,2,'2024-06-08','13:00:00',1,NULL),(173,6,2,'2024-06-08','15:00:00',1,NULL),(174,6,2,'2024-06-08','17:00:00',1,NULL),(175,1,2,'2024-06-08','19:00:00',1,NULL),(176,7,2,'2024-06-08','22:00:00',1,NULL),(177,7,3,'2024-06-08','09:00:00',1,NULL),(178,4,3,'2024-06-08','11:00:00',1,NULL),(179,6,3,'2024-06-08','13:00:00',1,NULL),(180,2,3,'2024-06-08','15:00:00',1,NULL),(181,3,3,'2024-06-08','18:00:00',1,NULL),(182,3,3,'2024-06-08','21:00:00',1,NULL),(183,8,1,'2024-06-09','09:00:00',1,NULL),(184,7,1,'2024-06-09','11:00:00',1,NULL),(185,2,1,'2024-06-09','13:00:00',1,NULL),(186,6,1,'2024-06-09','16:00:00',1,NULL),(187,7,1,'2024-06-09','18:00:00',1,NULL),(188,4,1,'2024-06-09','20:00:00',1,NULL),(189,5,1,'2024-06-09','22:00:00',1,NULL),(190,2,2,'2024-06-09','09:00:00',1,NULL),(191,8,2,'2024-06-09','12:00:00',1,NULL),(192,2,2,'2024-06-09','14:00:00',1,NULL),(193,6,2,'2024-06-09','17:00:00',1,NULL),(194,2,2,'2024-06-09','19:00:00',1,NULL),(195,4,2,'2024-06-09','22:00:00',1,NULL),(196,7,3,'2024-06-09','09:00:00',1,NULL),(197,3,3,'2024-06-09','11:00:00',1,NULL),(198,8,3,'2024-06-09','14:00:00',1,NULL),(199,4,3,'2024-06-09','16:00:00',1,NULL),(200,8,3,'2024-06-09','18:00:00',1,NULL),(201,2,3,'2024-06-09','20:00:00',1,NULL),(202,3,3,'2024-06-09','23:00:00',1,NULL);
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

-- Dump completed on 2024-06-06 13:12:25