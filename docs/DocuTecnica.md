---
title:
  Instituto Tecnológico de Costa Rica\endgraf\bigskip \endgraf\bigskip\bigskip\
  TecAir Proyecto 1: TECAir \endgraf\bigskip\bigskip\bigskip\bigskip
author:
  - José Morales Vargas, carné 2019024270
  - Alejandro Soto Chacón, carné 2019008164
  - Ignacio Vargas Campos, carné 2019053776
  - José Retana Corrales, carné 2020144743
date: \bigskip\bigskip\bigskip\bigskip Área Académica de\endgraf Ingeniería en Computadores \endgraf\bigskip\bigskip\ Bases de Datos \endgraf  (CE3101) \endgraf\bigskip\bigskip Profesor Marco Rivera Meneses \endgraf\vfill  Semestre I 2022
header-includes:
  - \setlength\parindent{24pt}
  - \usepackage{url}
  - \usepackage{float}
  - \floatplacement{figure}{H}
lang: es-ES
papersize: letter
classoption: fleqn
geometry: margin=1in
fontsize: 12pt
fontfamily: sans
linestretch: 1.5
bibliography: bibliografia.bib
csl: ieee.csl
nocite: | 
  @microsoft-2022A, @microsoft-2022B, @microsoft-2020, @google-2021, @google-2022, @elmasri-2016, @unknown-author-2021, @android-room
...

\maketitle
\thispagestyle{empty}
\clearpage
\tableofcontents
\pagenumbering{roman}
\clearpage
\pagenumbering{arabic}
\setcounter{page}{1}

## Modelo conceptual

## Modelo relacional

### Justificación de mapeo

## Estructuras de datos desarrolladas

### Servidor


### App móvil

En el caso de la aplicación móvil solo era necesario tener una imagen similar a la del servidor de los datos, sin embargo, se podía omitir varias relaciones presentes en la base de datos principal pues no son relevantes para la funcionalidad de la aplicación móvil. 

Afortunadamente, la biblioteca de Room permite definir las relaciones implementadas de forma concisa, por lo que se muestran extractos del código utilizado para representar la estructura de las relaciones implementadas. Nótese que existen relaciones similares a las del servidor, sin embargo, se han reducido datos almacenados y se ha cambiado campos para adaptar el modelo al contexto de la app móvil.

- Relación `User`: representación de un usuario. 

```Kotlin
@Entity(primaryKeys = ["username", "id"])
data class User(
    val type: Int = 0,
    val id: String, //uuid
    val username: String,
    val password: String,
    val firstName: String,
    val lastName: String,
    val phoneNumber: String,
    val email: String,
    val university: String,
    val studentId: String,
)
```

- Relación `UserOp`: representación de una operación pendiente a realizar
  sobre un usuario. La anotación `@Embedded` significa que se concatenan
  todos los datos de una tupla de la relación usuario. Para uso en sesiones
  sin conexión.

```Kotlin
@Entity
data class UserOp(
    @PrimaryKey val uuid: String,
    val operation: String,
    @Embedded
    val user: User
)
```

- Relación `Flight`: Contiene los datos que necesita la aplicación móvil sobre un vuelo.
  Se asume que el vuelo está en estado de booking.

```Kotlin
@Entity(primaryKeys = ["id"])
data class Flight(
    val id: String, //uuid
    val no: Int,
    val comment: String,
    val price: Double
)
```

- Relación `Flight`: Contiene los datos que necesita la aplicación móvil sobre un segmento
  de un vuelo. Se asume que solo son de vuelos en estado de booking.

```Kotlin
@Entity
data class Segment(
    @PrimaryKey val id: String,
    val flight: String,
    val seqNo: Int,
    val fromLoc: String, 
    val fromTime: String,
    val toLoc: String, 
    val toTime: String,
    val aircraft: String,
)
```

- Relación `Booking`: Utilizada para almacenar los datos de una reservación que se encuentra pendiente
  a concretar. Para uso en sesiones sin conexión. 

```Kotlin
@Entity(primaryKeys = ["flight", "pax"])
data class Booking(
    val flight: String, //uuid
    val pax: String, //uuid
    val promo: String //uuid
)
```

- Relación `Promo`: Contiene los datos relevantes para una promoción. Puede contener o no una imagen.

```Kotlin
@Entity
data class Promo(
    @PrimaryKey val id: String,
    val code: String,
    val flight: String,
    val price: Double,
    val startTime: Double,
    val endTime: String,
    @ColumnInfo(typeAffinity = ColumnInfo.BLOB)
    val img: ByteArray?,
)
```

## Descripción detallada de la arquitectura desarrollada

### Diagrama de arquitectura

![](imgs/diagramaArqui.png)

### Aplicación REST API

Para esta unidad de la arquitectura se tienen 3 elementos principales a notar, los cuales se ven denotados en el diagrama de arquitectura:

+ Se despliega la aplicación sobre una máquina virtual con Windows 10 que funciona como servidor de IIS.
+ El computador que funciona como servidor tiene instalado PostgreSQL como base de datos
+ En la capa del servicio en sí, se tiene una aplicación de C# que interactúa con la base de datos del computador utilizando
  el EntityFramework

El servicio web se ve expuesto en el puerto 5000 del servidor. 

### Aplicación Web

Para la unidad de aplicación web, similar a la REST API la misma es desplegada sobre un servidor de IIS, sin embargo, funcionalmente esto no significa que la app web "reside" en el servidor, sino que funciona como una fuente para enviar los recursos necesarios para construír la página a un usuario. 

Un usuario puede interactuar con dos vistas distintas: Vista de Reservaciones y Vista de Aeropuertos. Algunas partes de la funcionalidad es compartida, sin embargo existen funcionalidades en que son únicas a la Vista de Aeropuertos, esto porque esta "Sub Aplicación" de la aplicación principal tiene como usuario a los trabajadores de la aerolínea, mientras que la vista de reservaciones tiene como usuarios a los clientes. 
Esta estructura se puede notar en el diagrama de arquitectura, en donde se muestra que los recursos de la app web viven en el mismo servidor de IIS que la REST API, pero las vistas de operación se utilizan desde computadores individuales por aparte.

### Aplicación Móvil

En en caso de la app móvil se puede notar lo siguiente:

- Se hace uso de SQLite para guardar datos persistentes en el dispositivo móvil.
- La interfaz entre la aplicación y SQLite se da por medio de la biblioteca Room.
- El dispositivo móvil se conecta a la red, pero antes de conectarse a la red existe un nodo que indica la existencia de un proceso de sincronización durante las interacciones con los recursos de la red.

La arquitectura anterior es producto de los requerimientos de la aplicación móvil, en específico, que a diferencia de la aplicación web, la aplicación móvil debe proveer la funcionalidad de la vista de reservaciones incluso sin conexión a los recursos de red. Es por esto que la misma dispone de sus propias bases de datos locales. El tamaño de los símbolos de base de datos también denota algo adicional - respecto a la base de datos principal, la base de datos local de la aplicación móvil contiene significativamente menor información, puesto que mucha de la información guardada en el servidor principal es irrelevante para la operación de la aplicación móvil.

## Problemas conocidos

## Problemas encontrados

### Configuración de variables de ambiente en IIS: 

Para permitir que la REST API se pudiese desplegar en varios sistemas distintos se decidió hacer del connection string un dato provisto en la máquina del cliente. Inicialmente se creyó que las aplicaciones desplegadas en IIS tomarían valores de variables de ambiente del sistema que contiene el IIS, sin embargo esto resultó ser equivoco. 

- Resolución: Se siguieron los pasos indicados por [@unkown-author-2015] para la configuración de variables de ambiente en IIS.

### Solicitudes a HTTP con "clear text"

El problema encontrado consistía en que la aplicación móvil no podía realizar solicitudes al servidor, puesto que se hacía por medio de HTTP, no HTTPS.

- Resolución: Modificar el Android manifest para permitir este tipo de solicitudes de forma explícita:

```xml
    <application
        android:name=".TECAirApp"
        android:allowBackup="true"
        android:icon="@mipmap/ic_launcher"
        android:label="@string/app_name"
        android:roundIcon="@mipmap/ic_launcher_round"
        android:supportsRtl="true"
        android:theme="@style/Theme.TECAir"
        android:usesCleartextTraffic="true">
```

## Conclusiones

## Recomendaciones

## Bibliografía

::: {#refs}
:::

## Diagramas de clases

![](imgs/diagramaClasesAppServer.png)

![](imgs/diagramaClasesAppMovil.png)

