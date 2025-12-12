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
5. [__Descrição das Classes e Técnicas___](#descricao)
   - [__Path-Finding (A*)__](#path)
   - [__Decision Tree__](#decision)
   - [__Finite State Machine (FSM)__](#fsm)
6. [__Aspetos Técnicos e Decisões___](#aspetos)
7. [__Avaliação e Testes da IA___](#avaliacao)
8. [__Limitações e Trabalho Futuro__](#limitacoes)
9. [__Recursos Visuais___](#recursos)
10. [__Conclusão__](#Conclusão)

# __Introdução__
Este projeto apresenta a implementação de três técnicas de Inteligência Artificial aplicadas a NPCs num jogo RPG Roguelike 2.5D, desenvolvido em Unity.
O trabalho segue as orientações da Unidade Curricular Inteligência Artificial Aplicada a Jogos, que aborda técnicas como grafos, path-finding, máquinas de estados e árvores de decisão.
O objetivo da IA neste projeto é criar agentes inimigos capazes de:
- navegar de forma autónoma pelo ambiente
- tomar decisões com base no contexto,
- reagir ao jogador através de comportamento adaptado.

A arquitetura final integra três técnicas principais:
- Path-Finding (A)* – Navegação inteligente
- Decision Tree – Tomada de decisão de alto nível
- Finite State Machine (FSM) – Controlo de comportamento
  
<p align="center"> <img src="LINK_PARA_CAPA_DO_JOGO.png" width="800" alt="Gameplay Overview"> </p>
<a name="estrutura"></a>

# __Estrutura de Pastas__
* __Assets/__
    - __Scripts/__
        - __AI/__
            - [|-- AStarGrid.cs](#astar)
            - [|-- EnemyDecisionTree.cs](#decisiontree)
            - [|-- EnemyController.cs](#enemycontroller)
            - __EnemyStates/__
                - [|-- EnemyPatrolState.cs](#patrolstate)
                - [|-- EnemyChaseState.cs](#chasestate)
                - [|-- EnemyAttackState.cs](#attackstate)
                - [|-- EnemyFleeState.cs](#fleestate)
<a name="objetivo"></a>
## __Objetivo__
O jogador deve explorar masmorras geradas proceduralmente, recolher recursos e sobreviver a vários tipos de inimigos. A IA controla o comportamento desses inimigos, permitindo patrulhar, perseguir, atacar ou fugir conforme o estado.

<a name="controlos"></a>
## __Controlos__
- W, A, S, D: Movimentação.
- Rato (Botão Esquerdo): Ataque básico.
- Espaço: Dash / Esquiva.

<a name="jogabilidade"></a>
## __Jogabilidade__
A jogabilidade centra-se no combate e posicionamento. A IA utiliza o cenário a seu favor, evitando obstáculos e navegando por salas através do algoritmo A*.


<a name="instru"></a>
# __Instruções de Jogo__

1. Inicie o jogo no Menu Principal.

2. Explore a primeira sala segura.

3. Ao encontrar inimigos:

    - Se não virem o jogador → continuam em Patrulha

    - Se detetarem o jogador → entram em Perseguição

    - Se sofrerem dano e ficarem com pouca vida → ativam Fuga


<a name="arquitetura"></a>
# __Arquitetura de IA__
A arquitetura foi desenhada para ser modular, escalável e fácil de depurar, seguindo o modelo recomendado na UC para desenvolvimento de agentes inteligentes. 
Fluxo Geral
<p align="center"> <img src="https://i.postimg.cc/W1cztKPm/image123.png" width="800" alt="Gameplay Overview"> </p>

<a name="pipeline"></a>
## __Pipeline de Decisão__
Conforme ilustrado nos diagramas do projeto:
1. Decision Tree (Alto Nível): Avalia distância ao jogador, vida atual, linha de visão, etc.
2. FSM (Controlo): Ativa estados concretos: Patrulha, Perseguição, Ataque, Fuga.
3. A* (Navegação): Movimenta o inimigo com base no caminho calculado.

<a name="descricao"></a>
# __Descrição das Classes e Técnicas__

<a name="path"></a>
## __Path-Finding (A*)__
Implementado em AStarGrid.cs. O sistema de navegação discretiza o mapa numa grelha. O algoritmo A* é utilizado pelos estados de Patrulha, Perseguição e Fuga para calcular rotas eficientes, garantindo que os inimigos não ficam presos em paredes ou obstáculos.

<a name="decision"></a>
## __Decision Tree__
Implementada em EnemyDecisionTree.cs. Esta classe processa as variáveis do jogo e retorna uma Decision (Enum). A estrutura lógica segue a prioridade:

1. Sobrevivência: Se Health < 25% → Decisão: Flee.

2. Combate: Se Distance < AttackRange → Decisão: Attack.

3. Perseguição: Se Visible == True → Decisão: Chase.

4. Padrão: Caso contrário → Decisão: Patrol.

```
if (health < 25%) return Flee;
if (dist < 1.5) return Attack;
if (dist < 8) return Chase;
return Patrol;
```
<a name="fsm"></a>
## __Finite State Machine (FSM)__
Implementada através de classes de estado em Scripts/AI/EnemyStates/. A FSM recebe a decisão da Árvore e transita para o estado concreto. Cada estado (EnemyPatrolState, EnemyChaseState, etc.) é uma classe isolada que implementa a lógica específica de execução.
Foi criada uma FSM modular com estados individuais:

- EnemyPatrolState

- EnemyChaseState

- EnemyAttackState

- EnemyFleeState

```
Enter()
Update()
Exit()
```

<a name="aspetos"></a>
# __Aspetos Técnicos e Decisões__
- Separação de Responsabilidades: A lógica de decidir (Decision Tree) está totalmente separada da lógica de agir (FSM). Isto permite alterar as regras de decisão sem partir a movimentação do inimigo.
- *Eficiência do A:** O caminho não é recalculado a cada frame. O sistema utiliza um timer ou deteção de movimento do alvo para recalcular rotas apenas quando necessário, otimizando a performance.
- Visual Debugging: Foram implementados Gizmos no Unity para visualizar a grelha de navegação (nós caminháveis a branco, obstáculos a vermelho) e o caminho atual do inimigo.

<a name="avaliacao"></a>
# __Avaliação e Testes da IA__
Para validar o comportamento da IA, foram realizados vários testes:

- __Teste de Patrulha__
  - Verificar se navega corretamente entre waypoints
  - Garantir que evita paredes
  - Confirmar que muda para perseguição quando o jogador entra em visão

- __Teste de Perseguição__
  - Avaliar precisão da navegação usando A*
  - Confirmar recálculo do caminho ao movimento do jogador
  - Testar curvas apertadas e salas com obstáculos

- __Teste de Ataque__
  - Garantir que o inimigo só ataca dentro do alcance
  - Verificar tempos de cooldown

- __Teste de Fuga__
  - Confirmar que corre na direção oposta
  - Garantir que A* encontra um caminho viável para longe

- __Teste de Stress__
  - 20 inimigos simultâneos no mapa → FPS estável
  - Teste do recálculo do A* com múltiplos agentes
 
<a name="limitacoes"></a>
# __Limitações e Trabalho Futuro__
- __Limitações atuais__
  - O A* não usa multi-threading (não é ideal para dezenas de agentes).
  - A Decision Tree tem lógica fixa e pouco extensível sem editar código.
  - Os inimigos não comunicam entre si (sem cooperação / swarm behavior).

- __Melhorias futuras__
  - Implementar Behavior Trees completas.
  - Substituir A* por NavMesh gerado em runtime.
  - Adicionar ataques especiais, bosses e estados mais complexos.
  - Criar um sistema de memória para que inimigos “se lembrem” de locais vistos.

<a name="recursos"></a>
# __Recursos Visuais__

<a name="Conclusão"></a>
# __Conclusão__
A integração do A*, Decision Tree e FSM resultou numa IA modular, robusta e reativa , cumprindo todas as exigências da Unidade Curricular.

O sistema implementado permite criar agentes desafiantes e realistas, e serve como base sólida para futuras expansões, como comportamentos cooperativos, diferentes tipos de inimigos e maior profundidade estratégica.

Este trabalho demonstrou não só a aplicação prática dos algoritmos estudados, mas também técnicas reais usadas na indústria de jogos.
