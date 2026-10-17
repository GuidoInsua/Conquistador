```
 ##########################################################################
 ## o ########## o ##                                                     #
 ### \ ######## / ###                                                     #
 #### \ ###### / ####       C O N Q U I S T A D O R                       #
 ##### :-'""'-: #####       ----------------------                        #
 ## .-'  ____  `-. ##       Un juego de naves 2D hecho en Unity           #
 # ( (  (_()_)  ) ) #                                                     #
 ## `-.   ^^   .-' ##       Programacion 1 · 2do Cuatri 2026              #
 ##### `._==_.' #####                                                     #
 ###### __)(___ #####                                                     #
 ##########################################################################
```

# Conquistador

Shooter espacial 2D de pantalla fija. Manejás una nave, esquivás los disparos de los aliens, los destruís, juntás power-ups y al final enfrentás al jefe cangrejo. Si te quedás sin vida, perdés.

Trabajo práctico de **Programación 1** (segundo cuatrimestre 2026) de **Guido Insua**.

```
        .  *     .       +        .    *       .
    +       .        .        *          .
          \  |  /                 ___
       -- (  o  ) --          .-'  _  `-.        <- los aliens entran
          /  |  \            ( ( (_o_) ) )          por los costados
    *           .             `-._===_.-'
                                  |
          .                       |  <- disparo enemigo
                  +               v
     .                                          *
                       |
                       |  <- tu disparo
                       |
                      /^\
                     / | \
                    /_/_\_\
                    TU NAVE
```

## Cómo se juega

| Acción   | Teclado             |
| -------- | ------------------- |
| Moverte  | `W A S D` o flechas |
| Disparar | `Espacio`           |
| Pausa    | `Escape`            |
| Menús    | Flechas y `Enter`   |

Todo se juega con teclado: cada menú abre con su primera opción marcada.

### Aliens

- Tus disparos suben y hacen **1** de daño. El alien común tiene **4** de vida y el alien volador, **6**.
- Los aliens entran por el costado más cercano a su lugar haciendo ondas, y se quedan dando vueltas al azar alrededor de una de las **13** posiciones de la pantalla, así que puede haber hasta **13** a la vez. Aparece uno nuevo cada **2 segundos**.
- Hay **2 oleadas**, **50 aliens** en total: primero **20** comunes y después **30** mezclados (60 % comunes, 40 % voladores). La segunda no arranca hasta que mueren todos los de la primera.
- Cada alien dispara cada **6 segundos**. El común tira 1 disparo rápido; el volador tira **3 en abanico**, lentos, que caen balanceándose como un copo de nieve.
- Al morir, un alien tiene **80 %** de chances de soltar un power-up, elegido al azar entre los 4. Si pasa de largo y sale de la pantalla, se pierde:
  - **Amarillo**: **50 %** más de velocidad durante **10 segundos**.
  - **Verde**: +1 punto de vida.
  - **Rojo**: balas más anchas y con **2** de daño durante **20 segundos**.
  - **Azul**: disparo doble en paralelo durante **20 segundos**.

### Jefe cangrejo

Al terminar las oleadas cambia la música, la cámara se aleja un poco y baja el jefe. Te sigue despacio, moviéndose al azar.

- **Rayo:** cada **4 segundos** se frena, pone cara de enojado, avisa con un rayo fino y lanza desde la boca un rayo continuo que te saca **1** punto de vida por segundo mientras estés adentro.
- **Pinzas:** entre rayo y rayo, por turnos, cada pinza larga un abanico de **5** orbes de a uno, con una bola en la punta que avisa de dónde salen. Los orbes bajan balanceándose.
- **Esbirros:** cada **5 segundos** salen **4** de adentro del jefe, hacia los costados. Tienen **1** de vida, no disparan, bajan oscilando hacia vos y te quitan **1** al tocarte, rebotando para no seguir pegándote. Se empujan entre ellos para no encimarse. Si los matás pueden soltar un power-up (**25 %**); a los **16 segundos** se desarman solos, sin premio.
- **Vida:** **120** en el cuerpo, que muestra el daño en tres pasos, y **50** en cada pinza. Las pinzas se rompen por separado, muestran el daño en dos pasos, quedan destruidas a la vista, sueltan **2** power-ups al azar y dejan de disparar.
- **Muerte:** cuando le rompés el cuerpo se hincha, explota en una lluvia de partículas, mueren todos los esbirros y unos segundos después aparece la victoria.

### Vida, pantallas y sonido

- Empezás con **5** puntos de vida, que se ven como fichas abajo a la izquierda. Cada golpe te quita **1**. Después de un golpe titilás en blanco y durante **1 segundo** no recibís daño.
- **Menú principal:** Jugar, Opciones, Créditos y Salir. En Opciones hay un volumen para la música y otro para los efectos, que se guardan entre partidas. Créditos tiene una sinopsis corta y los datos del autor.
- **Pausa** (`Escape`): Continuar, Opciones, Menu y Salir. El juego queda congelado.
- **Victoria y derrota:** Reiniciar, Menu y Salir, cada una con su banner y su música.
- Hay música para el menú, el juego, el jefe, la victoria y la derrota, y efectos para tu disparo, cuando te pegan, cuando muere un enemigo o una parte del jefe, y para el rayo.

## Cómo correrlo

1. Instalá **Unity 6000.3.20f1** (Unity Hub → Installs).
2. Clonalo:
    ```
    git clone https://github.com/GuidoInsua/Conquistador.git
    ```
3. Abrí la carpeta con Unity Hub (Add → Add project from disk).
4. Abrí la escena `Assets/Scenes/Menu.unity` y dale a **Play**.

Las escenas del build son `Menu` y `Juego`. El juego está pensado para pantallas 16:9; en otras proporciones la cámara y los menús se adaptan para que no quede nada cortado ni fuera de la vista.

## Tecnología

- Unity 6 con **URP 2D**
- **Input System** (acciones en `Assets/Settings/InputActions.inputactions`; los menús usan las acciones de UI por defecto)
- **TextMeshPro** para el texto de la interfaz
- `ObjectPool` de Unity para los aliens y los esbirros
- `Awaitable` de Unity para el movimiento de entrada de los aliens; corrutinas para las secuencias del jefe
- `PlayerPrefs` para guardar el volumen

## Estructura del proyecto

```
Conquistador/
├── Assets/
│   ├── Animations/   Animaciones de aliens, disparos y power-ups; las del jefe en Jefe/
│   ├── Animators/    Controladores de animación; los del jefe en Jefe/
│   ├── Audio/        Musica/ y SFX/
│   ├── Materials/    Partículas y el destello blanco del jugador
│   ├── Prefabs/      Aliens, disparos, power-ups, partículas
│   │   ├── Jefe/         Jefe, rayo, bola, bala de las pinzas, esbirro y explosión
│   │   └── UI/           Botón de menú y panel de opciones (se reusan en las dos escenas)
│   ├── Scenes/       Menu y Juego
│   ├── Scripts/
│   │   ├── Audio/        Efectos de sonido, cambio de música y volumen
│   │   ├── Enemies/      Aliens, Spawner, Oleada y el jefe (FaseBoss, JefeCangrejo, ParteBoss, RayoBoss, DisparoPinza, Esbirro, InvocadorEsbirros)
│   │   ├── Interfaces/   IDamageable
│   │   ├── Player/       Movimiento, disparo y vida del jugador
│   │   ├── PowerUps/     Los 4 power-ups
│   │   ├── Projectiles/  Bala del jugador y bala enemiga
│   │   ├── UI/           Barra de vida, cambio de escena, pausa, opciones y selección con teclado
│   │   ├── Utilidades/   Sorteo con pesos y oscilación
│   │   └── World/        Bordes, fondo animado, utilidad de pantalla
│   ├── Settings/     Input y render pipeline
│   ├── Sprites/      Background, Boss, Enemies, Player, PowerUps, UI, VFX
│   └── Tests/EditMode/   Pruebas automáticas
└── ProjectSettings/
```

## Pruebas

En Unity: **Window → General → Test Runner → EditMode → Run All**.
Son **38** y cubren la vida y el sonido de muerte del alien, la vida e invulnerabilidad del jugador, el Spawner (posiciones, oleadas, punto de entrada, victoria y paso al jefe), las partes del jefe y sus power-ups, el rayo, el abanico de las pinzas, los esbirros (golpe, rebote, choque entre ellos y vida limitada), la oscilación y la onda de entrada, el ajuste de la cámara y de los puntos de entrada a la pantalla, el volumen, la pausa, el límite de efectos de sonido, el sorteo con pesos, la elección de power-ups y la bala potenciada.

## Ideas para seguir

```
 [ ] Puntaje y récord
 [x] Oleadas con enemigos distintos
 [x] Jefe final
 [ ] Más power-ups (escudo)
 [x] Efectos de sonido y música
 [x] Pausa
```

## Autor

```
   Guido Insua
   insua.guido@gmail.com
   Programación 1 · 2do Cuatrimestre 2026
```
