-- phpMyAdmin SQL Dump
-- version 4.7.4
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Czas generowania: 06 Lut 2018, 10:54
-- Wersja serwera: 5.7.19
-- Wersja PHP: 5.6.31

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Baza danych: `supergierka`
--

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `postacie`
--

DROP TABLE IF EXISTS `postacie`;
CREATE TABLE IF NOT EXISTS `postacie` (
  `IDPostaci` int(11) NOT NULL AUTO_INCREMENT,
  `IDuzytkownika` int(10) UNSIGNED NOT NULL,
  `ImiePostaci` varchar(30) NOT NULL,
  `IDProfesji` int(10) UNSIGNED NOT NULL,
  `Zycie` int(10) UNSIGNED NOT NULL,
  `Sila` int(10) UNSIGNED NOT NULL,
  `Poziom` int(10) UNSIGNED NOT NULL,
  `Doswiadczenie` double UNSIGNED NOT NULL,
  PRIMARY KEY (`IDPostaci`)
) ENGINE=MyISAM AUTO_INCREMENT=146 DEFAULT CHARSET=utf8;

--
-- Zrzut danych tabeli `postacie`
--

INSERT INTO `postacie` (`IDPostaci`, `IDuzytkownika`, `ImiePostaci`, `IDProfesji`, `Zycie`, `Sila`, `Poziom`, `Doswiadczenie`) VALUES
(101, 1, 'GDFG', 7, 670, 570, 46, 4655),
(116, 47, 'WJAD', 8, 0, 0, 1, 0),
(115, 47, 'MCAIEK', 6, 0, 0, 0, 0),
(130, 1, 'KWAKWA', 7, 100, 10, 1, 10),
(142, 52, 'asdasdasd', 7, 100, 10, 1, 0);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `potwory`
--

DROP TABLE IF EXISTS `potwory`;
CREATE TABLE IF NOT EXISTS `potwory` (
  `IDPotwora` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Nazwa` varchar(30) NOT NULL,
  `Sila` int(10) UNSIGNED NOT NULL,
  `Zycie` int(10) UNSIGNED NOT NULL,
  `Doswiadczenie` double UNSIGNED NOT NULL,
  PRIMARY KEY (`IDPotwora`)
) ENGINE=MyISAM AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

--
-- Zrzut danych tabeli `potwory`
--

INSERT INTO `potwory` (`IDPotwora`, `Nazwa`, `Sila`, `Zycie`, `Doswiadczenie`) VALUES
(1, 'Dzik', 30, 200, 50),
(2, 'Waz', 25, 100, 45),
(3, 'Zebacz', 30, 250, 115),
(4, 'Troll górski', 100, 5000, 5000.5),
(5, 'Osa', 15, 50, 10);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `profesja`
--

DROP TABLE IF EXISTS `profesja`;
CREATE TABLE IF NOT EXISTS `profesja` (
  `IDProfesji` int(11) NOT NULL AUTO_INCREMENT,
  `Nazwa` varchar(30) NOT NULL,
  `PoczatkoweZycie` int(10) UNSIGNED NOT NULL,
  `PoczatkowaSila` int(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`IDProfesji`)
) ENGINE=MyISAM AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;

--
-- Zrzut danych tabeli `profesja`
--

INSERT INTO `profesja` (`IDProfesji`, `Nazwa`, `PoczatkoweZycie`, `PoczatkowaSila`) VALUES
(7, 'Paladyn', 100, 10),
(6, 'Rogue', 90, 15),
(5, 'Warrior', 110, 15),
(1, 'Necro', 50, 5),
(8, 'Druid', 150, 5),
(9, 'God', 5000, 5000);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `przedmioty`
--

DROP TABLE IF EXISTS `przedmioty`;
CREATE TABLE IF NOT EXISTS `przedmioty` (
  `IDPrzedmiotu` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Nazwa` varchar(30) NOT NULL,
  `Zycie` int(10) UNSIGNED NOT NULL,
  `Sila` int(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`IDPrzedmiotu`)
) ENGINE=MyISAM AUTO_INCREMENT=100 DEFAULT CHARSET=utf8;

--
-- Zrzut danych tabeli `przedmioty`
--

INSERT INTO `przedmioty` (`IDPrzedmiotu`, `Nazwa`, `Zycie`, `Sila`) VALUES
(50, 'Siekiera', 10, 500),
(1, 'Grabie', 0, 5),
(99, 'Miecz', 0, 50);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `przedmiotypostaci`
--

DROP TABLE IF EXISTS `przedmiotypostaci`;
CREATE TABLE IF NOT EXISTS `przedmiotypostaci` (
  `IDPostaci` int(11) NOT NULL,
  `IDPrzedmiotu` int(11) NOT NULL,
  `ID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`IDPostaci`,`IDPrzedmiotu`,`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `przygody`
--

DROP TABLE IF EXISTS `przygody`;
CREATE TABLE IF NOT EXISTS `przygody` (
  `IDPrzygody` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Tytul` varchar(30) NOT NULL,
  `Opis` text NOT NULL,
  `Potwory` varchar(30) NOT NULL COMMENT 'Ilosc IDpotwora,Ilosc IDpotwora,Ilosc IDpotwora',
  `IDPrzedmiotuZaNagrode` int(10) UNSIGNED NOT NULL,
  `IloscExp` int(10) UNSIGNED NOT NULL,
  `WymaganyPoziom` int(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`IDPrzygody`,`Potwory`,`IDPrzedmiotuZaNagrode`),
  UNIQUE KEY `Tytul` (`Tytul`)
) ENGINE=MyISAM AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;

--
-- Zrzut danych tabeli `przygody`
--

INSERT INTO `przygody` (`IDPrzygody`, `Tytul`, `Opis`, `Potwory`, `IDPrzedmiotuZaNagrode`, `IloscExp`, `WymaganyPoziom`) VALUES
(1, 'Schnitzel', 'Farmer: \"Oh my lord, I\'ve got request to You. My herd is oppressed by wild boar. Please, help me!', '3 1', 1, 100, 1),
(3, 'Beast from the mountains', 'Adventurer: \"Hi! I\'m coming back from mountain. Do you know what the beast is there? It\'s a troll! It was trying to hunt me but I escaped. Do something with it!\"', '1 2', 10, 500, 5),
(4, 'QWERTY', 'ASDFASDASDASD', '1 1', 1, 100, 1),
(9, 'QWERTYsdfsdfsfd', 'ASDFASDASDASD', '1 1', 1, 100, 1),
(6, 'KATASTROFA', 'asdasd asda sda g we q w', '1 1,2 2,3 3,4 5 ', 1, 100, 1),
(7, 'KOZACKA PRZYGODA', 'to jest najlepsza przygoda', '1 1,3 3', 99, 300, 2),
(8, 'rew', 'asdasdasd', '1 1', 1, 100, 2);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `uzytkownik`
--

DROP TABLE IF EXISTS `uzytkownik`;
CREATE TABLE IF NOT EXISTS `uzytkownik` (
  `ID` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Login` varchar(30) NOT NULL,
  `Haslo` varchar(32) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=MyISAM AUTO_INCREMENT=59 DEFAULT CHARSET=utf8;

--
-- Zrzut danych tabeli `uzytkownik`
--

INSERT INTO `uzytkownik` (`ID`, `Login`, `Haslo`) VALUES
(1, 'Mac', '55AEF46853C5ACA2A0B05B502B767E11'),
(44, 'Maciej', 'B88E14944B6D94F21D4A8050D7DAE221'),
(45, 'Dwaid', 'CB124188613DB6C0FF30116467D1A400'),
(46, 'Karny', 'C164F4EE42FC4691D4460E4071505078'),
(47, 'KEICAM', '70F8E99E2E0089EF2DF1AD3FA8E7EF67'),
(54, 'aaaaaaaaaaaaaaa', 'A51F6CE1D7C1CF2A514D4BE660834713'),
(53, 'MACIEKTEST', '70F8E99E2E0089EF2DF1AD3FA8E7EF67'),
(52, 'MACIUSTEST', '70F8E99E2E0089EF2DF1AD3FA8E7EF67'),
(55, '<b>1`````````', 'A51F6CE1D7C1CF2A514D4BE660834713'),
(56, 'wwwwwwwwwwaaa', 'B4BC599DFD05A49E97D4655054B6D33F'),
(57, 'aaaa', 'C891B3F27B647F0849BEA0556EB89C33'),
(58, 'bbbbbbbbbbbbbbb', 'DD797768DD33ECED85E39DA9566E87C4');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `wykonaneprzygody`
--

DROP TABLE IF EXISTS `wykonaneprzygody`;
CREATE TABLE IF NOT EXISTS `wykonaneprzygody` (
  `IDPostaci` int(10) UNSIGNED NOT NULL,
  `IDPrzygody` int(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`IDPostaci`,`IDPrzygody`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Zrzut danych tabeli `wykonaneprzygody`
--

INSERT INTO `wykonaneprzygody` (`IDPostaci`, `IDPrzygody`) VALUES
(101, 1),
(101, 3),
(101, 4),
(101, 5),
(101, 7);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
