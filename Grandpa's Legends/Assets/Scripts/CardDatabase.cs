using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Cards> cardList = new List<Cards>();

    void Awake()
    {
        cardList.Add(new Cards(0, "None", 0, 0, 0, "None"));
        cardList.Add(new Cards(1, "Maria Caninana", 1, 1, 2, "None"));
        cardList.Add(new Cards(2, "Aticupu", 2, 1, 3, "Escolha uma carta aliada. Enquanto sobreviver, cura +1 de vida por rodada"));
        cardList.Add(new Cards(3, "Corpo Seco", 2, 2, 2, "Quando for derrotado, retorna para a mão do jogador"));
        cardList.Add(new Cards(4, "Cumadre Fulôzinha", 2, 1, 2, "Envenena o alvo de ataque por 2 turnos"));
        cardList.Add(new Cards(5, "Hipocampo", 3, 2, 3, "Escolha um inimigo e reduza seu ataque em 2"));
        cardList.Add(new Cards(6, "Boitatá", 3, 0, 6, "Aterroriza quem atacar, reduzindo seu dano em 2"));
        cardList.Add(new Cards(7, "Caipora", 3, 0, 3, "Concede um escudo de 3 a uma carta aliada a cada rodada"));
        cardList.Add(new Cards(8, "Curupira", 3, 3, 2, "Toda vez que atacar, ganha +1 de dano"));
        cardList.Add(new Cards(9, "Matinta Pereira", 3, 1, 2, "Ao entrar em jogo, compre 2 cartas de item."));
        cardList.Add(new Cards(10, "Onça-boi", 3, 2, 3, "A cada Onça-Boi em campo, ganha um buff de status"));
        cardList.Add(new Cards(11, "Zumbi da meia noite", 3, 1, 5, "Só é derrotado se receber dano suficiente para zerar sua vida de uma vez. Se sobreviver ao round, restaura toda a vida"));
        cardList.Add(new Cards(12, "Boiúna", 4, 2, 3, "Adiciona 2 cartas de Peixe à mão do jogador"));
        cardList.Add(new Cards(13, "Alamoa - Humana", 4, 2, 6, "Escolha 2 inimigos e reduza sua vida em -2 ao seduzi-los. Upgrade: Se executar 3 inimigos, evolui e ganha +2 de vida"));
        cardList.Add(new Cards(14, "Alamoa - Esqueleto", 4, 2, 8, "Cura 1 de vida ao causar dano em inimigos seduzidos"));
        cardList.Add(new Cards(15, "Cobra Norato", 4, 1, 3, "Ao entrar em campo, escolhe uma carta aliada para conceder Sobrevida (revive com 1 de vida se for derrotada). Se esta carta for eliminada antes da ressurreição, o efeito desaparece."));
        cardList.Add(new Cards(16, "Mula sem cabeça", 4, 2, 2, "Aplica queimadura por 3 turnos"));
        cardList.Add(new Cards(17, "Negrinho do Pastoreio", 5, 2, 1, "Após ser derrotada, revive com 4 de vida e 4 de dano. Passivamente, recupera 1 de vida por turno se não estiver com a vida máxima"));
        cardList.Add(new Cards(18, "Cuca", 5, 4, 4, "Rouba uma carta da mão do inimigo"));
        cardList.Add(new Cards(19, "Barba ruiva - Criança", 5, 0, 8, "Se sobreviver por 3 rodadas, evolui. Upgrade: ganha +5 de dano e recupera toda a vida"));
        cardList.Add(new Cards(20, "Barba ruiva - Adulto", 5, 5, 6, "None"));
        cardList.Add(new Cards(21, "Boi Vaquim", 5, 4, 2, "Ao entrar em campo, ataca imediatamente, escolhendo até 2 alvos (dano reduzido se atacar o inimigo diretamente)"));
        cardList.Add(new Cards(22, "Iara", 5, 2, 4, "Ao entrar em combate, escolhe uma carta inimiga para ser executada em 3 rodadas. Se for derrotada antes, o efeito é cancelado. Recebe -1 de dano da carta seduzida"));
        cardList.Add(new Cards(23, "Lobisomem - Homem", 5, 2, 4, "Para se transformar em Lobisomem, precisa sobreviver por 3 rodadas. Upgrade: Ganha +1 de dano e +2 de vida. "));
        cardList.Add(new Cards(24, "Lobisomem", 5, 3, 6, "Ao atacar cartas que não estão com a vida máxima, da +1 de dano."));
        cardList.Add(new Cards(25, "Boto Cor de Rosa ", 6, 3, 3, "A cada 2 turnos do inimigo, se esconde e não pode ser atacada. Upgrade: Causar 9 de dano"));
        cardList.Add(new Cards(26, "Boto Cor de Rosa - Homem", 6, 6, 4, "Reduz a vida do inimigo atacado em 2"));
        cardList.Add(new Cards(27, "Romãozinho", 6, 2, 2, "Reduz 1 de dano do inimigo. Retorna a mão do jogador após a morte. Fica mais forte sempre que um inimigo morre enquanto estiver em campo +1 de vida e dano (perde o buff ao reviver)."));
        cardList.Add(new Cards(28, "Kianumaka Manã - Humana", 7, 3, 9, "Cura 1 de vida sempre que eliminar uma carta inimiga. Upgrade: Em campo: ao ver 3 cartas aliadas morrerem "));
        cardList.Add(new Cards(29, "Kianumaka Manã - Onça", 7, 3, 9, "Ataca 2 cartas inimigas ao mesmo tempo, mas perde a autocura."));
        cardList.Add(new Cards(30, "Saci Pererê", 8, 4, 2, "Troca o dano pela vida das cartas inimigas em campo ao entrar na batalha. Atordoa uma carta alvo no campo."));
        cardList.Add(new Cards(31, "Esqueleto", 1, 2, 1, "None"));
        cardList.Add(new Cards(32, "Cobra", 1, 1, 2, "None"));
        cardList.Add(new Cards(33, "Onça", 3, 2, 4, "None"));
        cardList.Add(new Cards(34, "Lobo-guará", 3, 4, 2, "None"));
        cardList.Add(new Cards(35, "Arara azul", 2, 2, 2, "None"));
        cardList.Add(new Cards(36, "Sapo", 2, 3, 1, "None"));
        cardList.Add(new Cards(37, "Gavião real", 4, 5, 2, "None"));
        cardList.Add(new Cards(38, "Batata de purga", 3, 0, 0, "Remove todos os efeitos negativos(alvo-único) e cura em +1 de vida."));
        cardList.Add(new Cards(39, "Guaraná", 6, 0, 0, "Escolha uma carta aliada para aumentar seu dano em metade do valor do dano dessa carta e permitir que ataque duas vezes no mesmo turno."));
        cardList.Add(new Cards(40, "Inhame", 2, 0, 0, "Cura 1. O próximo dano recebido será reduzido em 1."));
        cardList.Add(new Cards(41, "Ipecacuanha", 7, 0, 0, "Concede imunidade a efeitos negativos para todo o campo por 2 turnos."));
        cardList.Add(new Cards(42, "Jerimum", 4, 0, 0, "Cura 1 para todas as cartas aliadas no campo"));
        cardList.Add(new Cards(43, "Milho", 1, 0, 0, "Cura 1"));
        cardList.Add(new Cards(44, "Vitória-régia", 3, 0, 0, "Cura 2"));
        cardList.Add(new Cards(45, "Peixe", 1, 2, 1, "None"));
    }
}
