# Trabalho Prático - Implementação de IA em RPG Roguelike 2.5D - Inteligência Artificial Aplicada a Jogos - [Marco Macedo] nº27919 / [João Reis] nº27917

# __Indíce__
1. [__Introdução__](#Introdução)
2. [__Estrutura de Pastas__](#estrutura) 
   - [__Objetivo__](#objetivo)
   - [__Controlos__](#controlos)
   - [__Jogabilidade__](#jogabilidade)
3. [__Instruções de Jogo__](#instru)
4. [__Arquitetura de IA__](#arquitetura)
   - [__Pipeline de Decisão__](#pipeline)
5. [__Descrição das Classes e Técnicas___](#aspetos)
   - [__Path-Finding (A*)__](#path)
   - [__Decision Tree__](#decision)
   - [__Finite State Machine (FSM)__](#fsm)
6. [__Aspetos Técnicos e Decisões___](#aspetos)
7. [__Recursos Visuais___](#recursos)
8. [__Conclusão__](#Conclusão)

# __Introdução__
Este projeto apresenta a implementação de um sistema de Inteligência Artificial para NPCs num jogo RPG Roguelike 2.5D, desenvolvido em Unity. O objetivo central é demonstrar a aplicação prática de conceitos lecionados na Unidade Curricular, especificamente o estudo da arquitetura e design de agentes inteligentes, comportamento autônomo e tomada de decisão.
O sistema substitui comportamentos básicos por uma arquitetura integrada composta por três técnicas obrigatórias: *Path-Finding (A)**, Decision Trees e Finite State Machines (FSM).


<p align="center"> <img src="LINK_PARA_CAPA_DO_JOGO.png" width="800" alt="Gameplay Overview"> </p>
<a name="estrutura"></a>

# __Estrutura de Pastas__

<a name="objetivo"></a>
## __Objetivo__
O jogador deve explorar masmorras geradas proceduralmente, recolher recursos e sobreviver. O objetivo da IA é proporcionar oponentes que não apenas seguem o jogador, mas que tomam decisões táticas (fugir, patrulhar, atacar) com base no seu estado atual.

<a name="controlos"></a>
## __Controlos__
- W, A, S, D: Movimentação.
- Rato (Botão Esq.): Ataque básico.
- Espaço: Dash / Esquiva.

<a name="jogabilidade"></a>
## __Jogabilidade__
A jogabilidade foca-se no combate e posicionamento. A IA utiliza o cenário a seu favor, navegando por corredores e evitando obstáculos para alcançar ou fugir do jogador.



<a name="instru"></a>
# __Instruções de Jogo__

1. Inicie o jogo no Menu Principal.

2. Navegue pela sala inicial segura.

3. Ao encontrar inimigos, observe os seus padrões:

    - Se o inimigo não o vir, ele manterá a Patrulha.

    - Se for detetado, ele iniciará a Perseguição.

    - Se causar dano suficiente, o inimigo entrará em estado de Fuga.


<a name="arquitetura"></a>
# __Arquitetura de IA__
A arquitetura dos inimigos foi desenhada num sistema em cascata, garantindo modularidade e fácil depuração.


<a name="pipeline"></a>
## __Pipeline de Decisão__
Conforme ilustrado nos diagramas do projeto:
1. Decision Tree (Alto Nível): O "cérebro" que avalia o contexto (Vida, Distância, Visão).
2. FSM (Controlo): O gestor que ativa o comportamento correspondente à decisão.
3. *A (Navegação):** O sistema utilitário que calcula o movimento físico no mapa.

## __Estrutura de Pastas__

Camada de Decisão (Decision Tree): O inimigo avalia o contexto (vida, distância do jogador, visão).

Camada de Controlo (FSM): Com base na decisão, o estado comportamental é alterado (ex: Patrulhar, Perseguir).

*Camada de Navegação (A):** O estado ativo solicita um caminho ao sistema de navegação para se mover no cenário evitando obstáculos.
