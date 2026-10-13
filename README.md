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

Shooter espacial 2D de pantalla fija. Manejás una nave, esquivás los disparos de los aliens, los destruís y juntás power-ups para ir más rápido. Si te alcanza un disparo enemigo, perdés.

Trabajo práctico de **Programación 1** (segundo cuatrimestre 2026) de **Guido Insua**.

```
        .  *     .       +        .    *       .
    +       .        .        *          .
          \  |  /                 ___
       -- (  o  ) --          .-'  _  `-.        <- los aliens entran
          /  |  \            ( ( (_o_) ) )          desde arriba
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

- Tus disparos suben y hacen **1** de daño. El alien común tiene **4** de vida y el alien volador, **6**.
- Los aliens salen desde arriba hasta quedarse en sus posiciones. Puede haber hasta **10** a la vez y aparece uno nuevo cada **2 segundos**.
- Hay **2 oleadas**, **60 aliens** en total: primero **20** comunes y después **40** mezclados (60 % comunes, 40 % voladores). Si los matás a todos, ganás.
- Cada alien dispara cada **6 segundos**. El común tira 1 bala; el volador tira **3 en abanico**, lentas, que caen balanceándose como un copo de nieve.
- Empezás con **5** puntos de vida, que se ven como fichas abajo a la izquierda. Cada disparo enemigo te quita **1**.
- Al morir un alien hay chances de que suelte un power-up, y se elige uno de los 4 al azar. Si pasa de largo y sale de la pantalla, se pierde:
  - **Amarillo**: más velocidad durante un rato.
  - **Verde**: +1 punto de vida.
  - **Rojo**: balas más anchas y con **2** de daño durante **20 segundos**.
  - **Azul**: disparo doble en paralelo durante **20 segundos**.
- Cuando te quedás sin vida aparece la pantalla de derrota, y si matás a los 60 aliens, la de victoria. Desde ahí reiniciás, volvés al menú o salís.

## Cómo correrlo

1. Instalá **Unity 6000.3.20f1** (Unity Hub → Installs).
2. Clonalo:
    ```
    git clone https://github.com/GuidoInsua/Conquistador.git
    ```
3. Abrí la carpeta con Unity Hub (Add → Add project from disk).
4. Abrí la escena `Assets/Scenes/Menu.unity` y dale a **Play**.

Las escenas del build son `Menu` y `Juego`.

## Tecnología

- Unity 6 con **URP 2D**
- **Input System** (acciones en `Assets/Settings/InputActions.inputactions`)
- **TextMeshPro** para el texto de la interfaz
- `ObjectPool` de Unity para los aliens
- `Awaitable` de Unity para el movimiento de entrada de los aliens

## Estructura del proyecto

```
Conquistador/
├── Assets/
│   ├── Animations/   Animaciones de aliens, disparos y power-ups
│   ├── Animators/    Controladores de animación
│   ├── Audio/        Música y efectos
│   ├── Materials/
│   ├── Prefabs/      Alien, láser, disparo enemigo, power-up, partículas
│   ├── Scenes/       Menu y Juego
│   ├── Scripts/
│   │   ├── Audio/        Música de fondo
│   │   ├── Enemies/      EnemigoAlien, DisparoEnemigo, Spawner
│   │   ├── Interfaces/   IDamageable
│   │   ├── Player/       Movimiento, disparo y vida del jugador
│   │   ├── PowerUps/     Los 4 power-ups
│   │   ├── Projectiles/  Bala del jugador y bala enemiga
│   │   ├── UI/           Menú principal, barra de vida y pantallas de fin
│   │   ├── Utilidades/   Sorteo con pesos
│   │   └── World/        Bordes, fondo animado, utilidad de pantalla
│   ├── Settings/     Input y render pipeline
│   ├── Sprites/
│   └── Tests/EditMode/   Pruebas automáticas
└── ProjectSettings/
```

## Pruebas

En Unity: **Window → General → Test Runner → EditMode → Run All**.
Hoy cubren la vida del alien (muere a 0 y no baja de ahí) y la vida del jugador (siempre entre 0 y el máximo).

## Ideas para seguir

```
 [ ] Puntaje y récord
 [x] Oleadas con enemigos distintos
 [ ] Jefe final
 [ ] Más power-ups (escudo)
 [ ] Efectos de sonido al disparar y al morir
 [ ] Pausa
```

## Autor

```
   Guido Insua
   insua.guido@gmail.com
   Programación 1 · 2do Cuatrimestre 2026
```
