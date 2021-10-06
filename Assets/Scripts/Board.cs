using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private List<Piece> pieces_list = new List<Piece>();

    private List<Transform> yellow_square_colored_orange = new List<Transform>();
    private List<Transform> green_square_colored_orange = new List<Transform>();

    // Some particular materials are used to higlight the square selected (the correspondent material is assigned elsewhere).
    public Material selected_object_material;
    public Material orange_color_1;
    public Material orange_color_2;
    public Material moving_piece_material;
    public Material orange_color_special_moves;

    public Material blue_king_check_material;

    public Material green_board;
    public Material yellow_board;

    // Material of the previous selected square and the identifyer of the square are passed on at each update.
    private Material original_material = null;
    private Material original_material_purple_square = null;
    private Material original_material_blue_king_check=null;

    private Transform previous_square = null;
    private Transform selected_piece_square = null;

    Renderer king_in_chek_renderer=null;

    // Graphical rappresentation of the pieces (only the ones in which a pawn can trasform into).
    // Values defined outside of the script.
    public GameObject white_queen_object;
    public GameObject black_queen_object;
    public GameObject white_rook_object;
    public GameObject black_rook_object;
    public GameObject white_bishop_object;
    public GameObject black_bishop_object;
    public GameObject white_knight_object;
    public GameObject black_knight_object;

    // the special_move_square is the variable that identifies the square skipped by a pawn when it moves two squares up on its first move.
    private int special_move_square;

    // white_turn indicate which color has to move next (if true is white, if false is balck).
    private bool white_turn;

    // variable refers to the choiche menu to choose to which type of piece upgrade the pawn when it arrives in the last square of the board.
    public Transform UI_pawn_upgrade;
    private Piece piece_to_update;
    private bool menu_is_open = false;

    public Transform UI_game_ended;
    public Transform UI_game_realy_ended;
    public Transform UI_camera_menu;
    public Transform UI_settings_menu;
    public Transform UI_fifty_message;
    public Text text;

    public Camera cameraA;
    public Camera cameraB;
    public Camera cameraC;
    public Camera cameraD;
    public Camera cameraE;
    public Camera cameraF;

    // After 75 moves without pieces eaten or pawn moved the game is ended with no winner, n_moves keep trak of the number of moves missing from 75.
    public int n_moves;
    // the 75 moves will be put in action only if the following variable will be true (the user can turn it to false).
    public bool n_moves_implemented;

    // Start is called before the first frame update
    void Start()
    {
        this.UI_pawn_upgrade.gameObject.SetActive(false);
        this.UI_game_ended.gameObject.SetActive(false);
        this.UI_game_realy_ended.gameObject.SetActive(false);
        this.UI_camera_menu.gameObject.SetActive(false);
        this.UI_settings_menu.gameObject.SetActive(false);
        this.UI_fifty_message.gameObject.SetActive(false);

        this.cameraA.enabled=true;
        this.cameraB.enabled=false;
        this.cameraC.enabled=false;
        this.cameraD.enabled=false;
        this.cameraE.enabled=false;
        this.cameraF.enabled=false;

        this.n_moves = 0;
        this.n_moves_implemented = true;

        white_turn = true;

        //identify withe pieces.
        pieces_list.Add(new Piece(28, "pawn", true, "white_pawn (5)"));
        pieces_list.Add(new Piece(27, "pawn", true, "white_pawn"));
        pieces_list.Add(new Piece(26, "pawn", true, "white_pawn (4)"));
        pieces_list.Add(new Piece(25, "pawn", true, "white_pawn (2)"));
        pieces_list.Add(new Piece(24, "pawn", true, "white_pawn (3)"));
        pieces_list.Add(new Piece(23, "pawn", true, "white_pawn (6)"));
        pieces_list.Add(new Piece(22, "pawn", true, "white_pawn (1)"));
        pieces_list.Add(new Piece(21, "pawn", true, "white_pawn (7)"));
        pieces_list.Add(new Piece(18, "rook", true, "white_tower"));
        pieces_list.Add(new Piece(17, "knight", true, "white_knight (2)"));
        pieces_list.Add(new Piece(16, "bishop", true, "white_bishop"));
        pieces_list.Add(new Piece(15, "queen", true, "white_queen"));
        pieces_list.Add(new Piece(14, "king", true, "white_king"));
        pieces_list.Add(new Piece(13, "bishop", true, "white_bishop (1)"));
        pieces_list.Add(new Piece(12, "knight", true, "white_knight (3)"));
        pieces_list.Add(new Piece(11, "rook", true, "white_tower (1)"));

        //identify black pieces.
        pieces_list.Add(new Piece(78, "pawn", false, "black_pawn (6)"));
        pieces_list.Add(new Piece(77, "pawn", false, "black_pawn (3)"));
        pieces_list.Add(new Piece(76, "pawn", false, "black_pawn (4)"));
        pieces_list.Add(new Piece(75, "pawn", false, "black_pawn"));
        pieces_list.Add(new Piece(74, "pawn", false, "black_pawn (1)"));
        pieces_list.Add(new Piece(73, "pawn", false, "black_pawn (2)"));
        pieces_list.Add(new Piece(72, "pawn", false, "black_pawn (5)"));
        pieces_list.Add(new Piece(71, "pawn", false, "black_pawn (7)"));
        pieces_list.Add(new Piece(88, "rook", false, "black_tower (1)"));
        pieces_list.Add(new Piece(87, "knight", false, "black_knight (2)"));
        pieces_list.Add(new Piece(86, "bishop", false, "black_bishop"));
        pieces_list.Add(new Piece(85, "queen", false, "black_queen"));
        pieces_list.Add(new Piece(84, "king", false, "black_king"));
        pieces_list.Add(new Piece(83, "bishop", false, "black_bishop (1)"));
        pieces_list.Add(new Piece(82, "knight", false, "black_knight (3)"));
        pieces_list.Add(new Piece(81, "rook", false, "black_tower"));
    }

    // Update is called once per frame
    void Update()
    {
        if (!menu_is_open)
        {
            // check if the king is under attack.
            // If under check color its square in blue.
            // this portion of the code should be excuted only  the first time Update is called after the change of the turn (variable king_in_check_renderer set to null).
            if (king_in_chek_renderer == null)
            {
                for (int i = 0; i < pieces_list.Count; i++)
                {
                    Piece piece = this.pieces_list[i];

                    if (white_turn == piece.colorWhite() && piece.getPieceName() == "king")
                    {
                        king_in_chek_renderer = piece.get_position_square().GetComponent<Renderer>();
                        original_material_blue_king_check = king_in_chek_renderer.material;

                        if (this.check_square_under_attack(piece.getSquareName()))
                        {
                            king_in_chek_renderer.material = blue_king_check_material;
                        }

                        // check if the game is finished.
                        if (check_finished_game())
                        {
                            if (king_in_chek_renderer.material.name == blue_king_check_material.name+" (Instance)")
                            {
                                if (this.white_turn)
                                {
                                    this.text.text = "Vince il nero!";
                                }
                                else
                                {
                                    this.text.text = "Vince il bianco!";
                                }
                            }
                            else
                            {
                                this.text.text = "Pareggio!";
                            }
                            this.menu_end();
                        }

                        //check if there have been 50 moves without a capture or a pawn movement, if true signal the event to the user.
                        if (this.n_moves == 50 && this.n_moves_implemented)
                        {
                            this.text.text ="50 mosse senza avanzamento.";
                            this.menu_fifty();
                        }

                        //check if there have been more than 75 moves without a capture or a pawn movement, if true end the game.
                        if (this.n_moves >= 75 && this.n_moves_implemented)
                        {
                            this.text.text = "Pareggio!";
                            this.menu_end();
                        }
                    }
                }
            }

            Renderer square_renderer;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                // when the mouse is clicked on a square, higlighted or not, reset the modified squares on the board, and, if highlighted, perform the action associated to the square clicked.
                if (Input.GetMouseButtonDown(0) && previous_square != null)
                {

                    if (this.check_movable_piece(previous_square))
                    {
                        this.erase_previous_choices();

                        previous_square.GetComponent<Renderer>().material = moving_piece_material;
                        // don't change original material if the square clicked is aleady the one where there is the piece selected to move.
                        if (this.original_material.name != this.moving_piece_material.name + " (Instance)")
                        {
                            this.original_material_purple_square = this.original_material;
                        }

                        this.selected_piece_square = previous_square;
                        // avoid to recolor the previous square when highlighting another square.
                        this.previous_square = null;

                        this.color_possible_destinations(selected_piece_square);
                    }

                    if (selected_piece_square != null && this.check_possible_destinations(previous_square))
                    {
                        this.erase_previous_choices();

                        this.move_the_piece(previous_square, selected_piece_square);

                        // recolor the square of the king in check with is original material.

                        king_in_chek_renderer.material = original_material_blue_king_check;

                        // avoid to recolor in orange the previous selected square, alredy recoloured in with the original color.
                        this.previous_square = null;
                        this.selected_piece_square = null;

                        // change the turn (moving color), only if a menu is not open.
                        if (!menu_is_open)
                        {
                            // set original_material_blue_king_check to null, so in the next turn it will be checked again if the king is under attack.
                            this.original_material_blue_king_check = null;
                            king_in_chek_renderer = null;

                            this.white_turn = !this.white_turn;
                        }
                    }
                }

                // set back the original color of the previous selected square.
                if (previous_square != null)
                {
                    square_renderer = previous_square.GetComponent<Renderer>();
                    square_renderer.material = original_material;
                }

                // select hitten square.
                Transform current_square = hit.transform;
                square_renderer = current_square.GetComponent<Renderer>();

                // highlight only the squares under one of the pieces that can move, or of one of the possible destinations of the piece selected.
                if (this.check_movable_piece(current_square) || (square_renderer.material.name == orange_color_1.name + " (Instance)") || (square_renderer.material.name == orange_color_2.name + " (Instance)") || (square_renderer.material.name == orange_color_special_moves.name + " (Instance)"))
                {
                    // set the colur of the new higlighted square.
                    original_material = square_renderer.material;
                    square_renderer.material = selected_object_material;

                    // set current square as new previous square.
                    previous_square = current_square;
                }
                else
                {
                    previous_square = null;
                }
            }
        }
    }

    // recolor the modified squares of the board with their original color .
    void erase_previous_choices()
    {
        if (this.selected_piece_square != null)
        {
            this.selected_piece_square.GetComponent<Renderer>().material = original_material_purple_square;
        }

        // color the green squares.
        for (int i = 0; i < green_square_colored_orange.Count; i++)
        {
            this.green_square_colored_orange[i].GetComponent<Renderer>().material = green_board;
        }

        // color the yellow squares.
        for (int i = 0; i < yellow_square_colored_orange.Count; i++)
        {
            this.yellow_square_colored_orange[i].GetComponent<Renderer>().material = yellow_board;
        }

        this.green_square_colored_orange = new List<Transform>();
        this.yellow_square_colored_orange = new List<Transform>();
    }

    // check if there is a piece that can be moved on the square selected.
    bool check_movable_piece(Transform square_to_highlight, bool searching_white_piece)
    {
        bool can_be_highlighted = false;

        string square_name = square_to_highlight.gameObject.transform.parent.name;

        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece piece = this.pieces_list[i];

            if (square_name == piece.getSquareName())
            {
                bool piece_color = piece.colorWhite();
                can_be_highlighted = (searching_white_piece && piece_color) || (!(searching_white_piece) && !(piece_color));
            }
        }

        return can_be_highlighted;
    }

    bool check_movable_piece(Transform square_to_highlight)
    {
        bool searching_white_piece = this.white_turn;

        return check_movable_piece(square_to_highlight, searching_white_piece);
    }

    // check if there is a piece on the square selected.
    bool check_piece_presence(Transform destination_square)
    {

        string square_name = destination_square.gameObject.transform.parent.name;

        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece piece = this.pieces_list[i];

            if (square_name == piece.getSquareName())
            {
                return true;
            }
        }

        return false;
    }

    // check if there is a piece on the square selected.
    bool check_piece_presence(int destination_square)
    {
        string square;
        int square_number;

        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece piece = this.pieces_list[i];

            square = piece.getSquareName();
            square=square.Substring(square.Length - 2);
            square_number= int.Parse(square);

            if (destination_square == square_number)
            {
                return true;
            }
        }

        return false;
    }

    // check if the king of the moving color is on the square selected.
    bool check_own_king_presence(Transform destination_square)
    {

        string square_name = destination_square.gameObject.transform.parent.name;

        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece piece = this.pieces_list[i];
 
            if (piece.colorWhite() == this.white_turn && piece.getPieceName()=="king" && square_name == piece.getSquareName())
            {
                return true;
            }
        }

        return false;
    }

    bool check_possible_destinations(Transform destination_square)
    {
        // the position is reacheable if its color before to be hihglihted was one of the three orange (has been indicated as one of the possible moves for the piece selected).
        if (destination_square != null && ((original_material.name == this.orange_color_1.name + " (Instance)") || (original_material.name == this.orange_color_2.name + " (Instance)") || (original_material.name == this.orange_color_special_moves.name + " (Instance)")))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void move_the_piece(Transform arriving_square, Transform starting_square)
    {
        // get the number and the name that identify the destination square.
        string destination_square = arriving_square.parent.name;
        destination_square = destination_square.Substring(destination_square.Length - 2);
        int number_of_square = int.Parse(destination_square);
        int number_of_element_to_eliminate = -1;
        int line_number = number_of_square / 10;
        Piece selected_piece=null;
        Transform rook_starting_square;


        this.special_move_square=-1;

        //increase the value of n_moves.
        this.n_moves++;

        // if present eliminate the piece on the destination square.
        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece piece = this.pieces_list[i];

            if (arriving_square.parent.name == piece.getSquareName())
            {
                Destroy(piece.get_piece_GameObject());
                number_of_element_to_eliminate = i;
            }
        }

        if (number_of_element_to_eliminate != -1)
        {
            this.pieces_list.RemoveAt(number_of_element_to_eliminate);

            this.n_moves = 0;
        }

        // move the piece to destination.
        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece piece = this.pieces_list[i];

            if (starting_square.parent.name == piece.getSquareName())
            {
                selected_piece = piece;

                // if the piece is a pawn that moves two square for the first time store the skipped square in the special_move_square variable.
                if(piece.getPieceName()=="pawn" && piece.has_never_moved_before() && (line_number==4 || line_number==5))
                {
                    if (white_turn)
                    {
                        special_move_square = number_of_square - 10;
                    }
                    else
                    {
                        special_move_square = number_of_square + 10;
                    }
                }

                if (piece.getPieceName() == "pawn")
                {
                    this.n_moves = 0;
                }

                // move piece.
                    piece.move_to_square(number_of_square);

                // movement animation.
                // the castle move has a special routine, and it will not use this one.
                if (!(piece.getPieceName() == "king" && original_material.name == this.orange_color_special_moves.name + " (Instance)"))
                {
                    StartCoroutine(movement_routine(piece, starting_square, arriving_square));
                }

                // set that the piece has moved at least one time.
                piece.has_now_moved();

            }
        }

        // check if it's a special move.
        if (original_material.name == this.orange_color_special_moves.name + " (Instance)")
        {
            if (selected_piece.getPieceName() == "king")
            {
                // in case of castling the following cicle for will move the correct rook to its destination.
                for (int i = 0; i < pieces_list.Count; i++)
                {
                    Piece piece = this.pieces_list[i];

                    if (piece.getPieceName() == "rook" && ((piece.get_square_number() / 10) == line_number))
                    {
                        if ((number_of_square%10)<4 && (piece.get_square_number() % 10 )< 4) 
                        {
                            rook_starting_square = piece.get_position_square();

                            // near castle moves the tower in the third square of the row.
                            piece.move_to_square(line_number*10+3);

                            // moving animation.
                            StartCoroutine(castle_movement_routine(selected_piece, starting_square, arriving_square, piece, rook_starting_square, piece.get_position_square()));

                            // set that the piece has moved at least one time.
                            piece.has_now_moved();
                        }
                        if ((number_of_square % 10) >  4 && (piece.get_square_number() % 10) > 4)
                        {
                            rook_starting_square = piece.get_position_square();

                            // long castle moves the tower in the fifth square of the row.
                            piece.move_to_square(line_number*10 + 5);

                            // moving animation.
                            StartCoroutine(castle_movement_routine(selected_piece, starting_square, arriving_square, piece, rook_starting_square, piece.get_position_square()));

                            // set that the piece has moved at least one time.
                            piece.has_now_moved();
                        }
                    }
                }
            }

            if (selected_piece.getPieceName() == "pawn")
            {
                if (!this.white_turn)
                {
                    for (int i = 0; i < pieces_list.Count; i++)
                    {
                        Piece piece = this.pieces_list[i];

                        if (number_of_square + 10 == piece.get_square_number())
                        {
                            Destroy(piece.get_piece_GameObject());
                            number_of_element_to_eliminate = i;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < pieces_list.Count; i++)
                    {
                        Piece piece = this.pieces_list[i];

                        if (number_of_square - 10 == piece.get_square_number())
                        {
                            Destroy(piece.get_piece_GameObject());
                            number_of_element_to_eliminate = i;
                        }
                    }
                }
                if (number_of_element_to_eliminate != -1)
                {
                    this.pieces_list.RemoveAt(number_of_element_to_eliminate);
                }
            }
        }

        // if a pawn has reached the last line of the borard, it has to choose what type of piece to become from now on.
        // the menu with the different possibilities is shown.
        if(selected_piece.getPieceName()=="pawn"&& (line_number == 8 || line_number == 1) && (!(this.menu_is_open)))
        {
            this.menu_is_open = true;
            this.piece_to_update = selected_piece;
            this.UI_pawn_upgrade.gameObject.SetActive(true);
        }
    }

    bool check_possible_castle_nearside() 
    {
        // set the current square number of the targeted rook assuming that it's black turn, if it's white turn change it.
        int square=81;
        int square_line = 8;
        string square_name;

        if (white_turn)
        {
            square = 11;
            square_line = 1;
        }

        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece piece = this.pieces_list[i];
            int square_to_check=piece.get_square_number();

            // check that the rook has never moved before and if there are no pieces between the tower and the king.
            if ((square_to_check == square && !piece.has_never_moved_before())||((square_to_check / 10==square_line)&&(square_to_check%10>1)&&(square_to_check % 10 < 4)))
            {
                return false;
            }
        }

        // check if the free squares are under attack from the opponent pieces.
        for(int i=1; i<4; i++)
        {
            square_name = "board " + (square+i);
            if (this.check_square_under_attack(square_name))
            {
                return false;
            }
        }

        return true; 
    }

    bool check_possible_castle_longside() 
    {
        // set the current square number of the targeted rook assuming that it's black turn, if it's white turn change it.
        int square = 88;
        int square_line = 8;
        string square_name;

        if (white_turn)
        {
            square = 18;
            square_line = 1;
        }

        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece piece = this.pieces_list[i];
            int square_to_check = piece.get_square_number();

            // check that the rook has never moved before and if there are no pieces between the tower and the king.
            if ((square_to_check == square && !piece.has_never_moved_before()) || ((square_to_check / 10 == square_line) && (square_to_check % 10 > 4) && (square_to_check % 10 < 8)))
            {
                return false;
            }
        }

        // check if the free squares are under attack from the opponent pieces.
        for (int i = 1; i < 5; i++)
        {
            square_name = "board " + (square - i);
            if (this.check_square_under_attack(square_name))
            {
                return false;
            }
        }

        return true;
    }

    // once selected the square of a piece, color the reachable squares in orange.
    void color_possible_destinations(Transform square_of_piece_to_move)
    {
        // list of squares to color in orange.
        List<Transform> square_to_color = new List<Transform>();

        // find out the piece to move.
        Piece piece_to_move = null;

        // find out the square of the king.
        string king_square=null;

        string square_name = square_of_piece_to_move.gameObject.transform.parent.name;
        Renderer renderer;

        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece piece = this.pieces_list[i];

            if (square_name == piece.getSquareName())
            {
                piece_to_move = piece;
            }

            if(piece.getPieceName()=="king" && (this.white_turn == piece.colorWhite()))
            {
                king_square = piece.getSquareName();
            }
        }

        // color the squares that are reachable moving the pieces with the color orange, and insert them in the lists of the modified squares.

        int current_square_number;
        Transform destination_square;

        int iterator;
        bool stop;

        switch (piece_to_move.getPieceName())
        {
            case "pawn":

                // if the pawn is white move up on the board if it is black it moves down.
                if (piece_to_move.colorWhite())
                {
                    // move forward.
                    current_square_number = piece_to_move.get_square_number();
                    destination_square = GameObject.Find("board " + (current_square_number + 10)).transform.GetChild(0);

                    if (!check_piece_presence(destination_square) && not_king_check(king_square, destination_square, piece_to_move))
                    {
                        square_to_color.Add(destination_square);   
                    }

                    if (!check_piece_presence(destination_square) &&  piece_to_move.has_never_moved_before())
                    {
                        destination_square = GameObject.Find("board " + (current_square_number + 20)).transform.GetChild(0);

                        if (!check_piece_presence(destination_square) && not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }

                    // capture a pieces.
                    if (current_square_number % 10 < 8)
                    {
                        destination_square = GameObject.Find("board " + (current_square_number + 11)).transform.GetChild(0);
                        if (check_movable_piece(destination_square, false) && not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }

                        if ((current_square_number+11) == this.special_move_square && not_king_check(king_square, destination_square, piece_to_move))
                        {
                            renderer = destination_square.GetComponent<Renderer>();

                            // materials in the script have an " (Instance)" at the end of the name. 
                            if (renderer.material.name == this.yellow_board.name + " (Instance)")
                            {
                                this.yellow_square_colored_orange.Add(destination_square);
                            }
                            else
                            {
                                this.green_square_colored_orange.Add(destination_square);
                            }
                            renderer.material = orange_color_special_moves;
                        }
                    }

                    if (current_square_number % 10 > 1)
                    {
                        destination_square = GameObject.Find("board " + (current_square_number + 9)).transform.GetChild(0);
                        if (check_movable_piece(destination_square, false) && not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }

                        if ((current_square_number+9)==this.special_move_square && not_king_check(king_square, destination_square, piece_to_move))
                        {
                            renderer = destination_square.GetComponent<Renderer>();

                            // materials in the script have an " (Instance)" at the end of the name. 
                            if (renderer.material.name == this.yellow_board.name + " (Instance)")
                            {
                                this.yellow_square_colored_orange.Add(destination_square);
                            }
                            else
                            {
                                this.green_square_colored_orange.Add(destination_square);
                            }
                            renderer.material = orange_color_special_moves;
                        }
                    }
                }
                else
                {
                    // move forward.
                    current_square_number = piece_to_move.get_square_number();
                    destination_square = GameObject.Find("board " + (current_square_number - 10)).transform.GetChild(0);

                    if (!check_piece_presence(destination_square) && not_king_check(king_square, destination_square, piece_to_move))
                    {
                        square_to_color.Add(destination_square);
                    }

                    if (!check_piece_presence(destination_square) && piece_to_move.has_never_moved_before())
                    {
                        destination_square = GameObject.Find("board " + (current_square_number - 20)).transform.GetChild(0);

                        if (!check_piece_presence(destination_square) && not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }

                    // capture a pieces.
                    if (current_square_number % 10 < 8)
                    {
                        destination_square = GameObject.Find("board " + (current_square_number - 9)).transform.GetChild(0);
                        if (check_movable_piece(destination_square, true) && not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }

                        if ((current_square_number-9) == this.special_move_square && not_king_check(king_square, destination_square, piece_to_move))
                        {
                            renderer = destination_square.GetComponent<Renderer>();

                            // materials in the script have an " (Instance)" at the end of the name. 
                            if (renderer.material.name == this.yellow_board.name + " (Instance)")
                            {
                                this.yellow_square_colored_orange.Add(destination_square);
                            }
                            else
                            {
                                this.green_square_colored_orange.Add(destination_square);
                            }
                            renderer.material = orange_color_special_moves;
                        }
                    }

                    if (current_square_number % 10 > 1)
                    {
                        destination_square = GameObject.Find("board " + (current_square_number - 11)).transform.GetChild(0);
                        if (check_movable_piece(destination_square, true) && not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }

                        if ((current_square_number-11) == this.special_move_square && not_king_check(king_square, destination_square, piece_to_move))
                        {
                            renderer = destination_square.GetComponent<Renderer>();

                            // materials in the script have an " (Instance)" at the end of the name. 
                            if (renderer.material.name == this.yellow_board.name + " (Instance)")
                            {
                                this.yellow_square_colored_orange.Add(destination_square);
                            }
                            else
                            {
                                this.green_square_colored_orange.Add(destination_square);
                            }
                            renderer.material = orange_color_special_moves;
                        }
                    }
                }
                break;

            case "bishop":

                current_square_number = piece_to_move.get_square_number();
                iterator = 11;
                stop = false;

                while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        { 
                            square_to_color.Add(destination_square); 
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator + 11;
                }

                iterator = 9;
                stop = false;

                while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator + 9;
                }

                iterator = -9;
                stop = false;

                while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator - 9;
                }

                iterator = -11;
                stop = false;

                while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator - 11;
                }

                break;

            case "rook":

                current_square_number = piece_to_move.get_square_number();
                iterator = 1;
                stop = false;

                while (((current_square_number + iterator) % 10 <= 8) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator + 1;
                }

                iterator = -1;
                stop = false;

                while (((current_square_number + iterator) % 10 >= 1) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator - 1;
                }

                iterator = 10;
                stop = false;

                while (((current_square_number + iterator) / 10 <= 8) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator + 10;
                }

                iterator = -10;
                stop = false;

                while (((current_square_number + iterator) / 10 >= 1) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator - 10;
                }

                break;

            case "queen":

                current_square_number = piece_to_move.get_square_number();
                iterator = 11;
                stop = false;

                while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator + 11;
                }

                iterator = 9;
                stop = false;

                while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator + 9;
                }

                iterator = -9;
                stop = false;

                while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator - 9;
                }

                iterator = -11;
                stop = false;

                while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }

                    iterator = iterator - 11;
                }

                iterator = 1;
                stop = false;

                while (((current_square_number + iterator) % 10 <= 8) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator + 1;
                }

                iterator = -1;
                stop = false;

                while (((current_square_number + iterator) % 10 >= 1) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator - 1;
                }

                iterator = 10;
                stop = false;

                while (((current_square_number + iterator) / 10 <= 8) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator + 10;
                }

                iterator = -10;
                stop = false;

                while (((current_square_number + iterator) / 10 >= 1) && !stop)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_piece_presence(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                    else
                    {
                        stop = true;
                        if (!check_movable_piece(destination_square))
                        {
                            if (not_king_check(king_square, destination_square, piece_to_move))
                            {
                                square_to_color.Add(destination_square);
                            }
                        }
                    }
                    iterator = iterator - 10;
                }

                break;

            case "king":

                current_square_number = piece_to_move.get_square_number();

                iterator = 1;
                if ((current_square_number + iterator) % 10 <= 8)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                    {
                        square_to_color.Add(destination_square);
                    }
                }

                iterator = -1;
                if ((current_square_number + iterator) % 10 >= 1)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                    {
                        square_to_color.Add(destination_square);
                    }
                }

                iterator = -10;
                if ((current_square_number + iterator) / 10 >= 1)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                    {
                        square_to_color.Add(destination_square);
                    }
                }

                iterator = 10;
                if ((current_square_number + iterator) / 10 <= 8)
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                    {
                        square_to_color.Add(destination_square);
                    }
                }

                iterator = 11;
                if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 <= 8))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                    {
                        square_to_color.Add(destination_square);
                    }
                }

                iterator = 9;
                if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                    {
                        square_to_color.Add(destination_square);
                    }
                }

                iterator = -9;
                if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                    {
                        square_to_color.Add(destination_square);
                    }
                }

                iterator = -11;
                if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 >= 1))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                    {
                        square_to_color.Add(destination_square);
                    }
                }

                // check if it is possible to castle.
                if (piece_to_move.has_never_moved_before())
                {

                    iterator = -2;
                    // castle on the near side (uses the orange color only for special moves).
                    if (check_possible_castle_nearside())
                    {
                        destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);

                        renderer = destination_square.GetComponent<Renderer>();

                        // materials in the script have an " (Instance)" at the end of the name. 
                        if (renderer.material.name == this.yellow_board.name + " (Instance)")
                        {
                            this.yellow_square_colored_orange.Add(destination_square);
                        }
                        else
                        {
                            this.green_square_colored_orange.Add(destination_square);
                        }
                        renderer.material = orange_color_special_moves;
                    }


                    // castle on the far side (uses the orange color only for special moves).
                    iterator = 2;
                    if (check_possible_castle_longside())
                    {
                        destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);

                        renderer = destination_square.GetComponent<Renderer>();

                        // materials in the script have an " (Instance)" at the end of the name. 
                        if (renderer.material.name == this.yellow_board.name + " (Instance)")
                        {
                            this.yellow_square_colored_orange.Add(destination_square);
                        }
                        else
                        {
                            this.green_square_colored_orange.Add(destination_square);
                        }
                        renderer.material = orange_color_special_moves;
                    }
                }

                break;

            case "knight":
                current_square_number = piece_to_move.get_square_number();

                iterator = 21;
                if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 <= 8))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                }

                iterator = 12;
                if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                }

                iterator = 19;
                if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                }

                iterator = 8;
                if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                }

                iterator = -8;
                if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                }

                iterator = -19;
                if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                }

                iterator = -21;
                if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 >= 1))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                }

                iterator = -12;
                if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                {
                    destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                    if (!check_movable_piece(destination_square))
                    {
                        if (not_king_check(king_square, destination_square, piece_to_move))
                        {
                            square_to_color.Add(destination_square);
                        }
                    }
                }

                break;
        }

        // color the squares the piece can move to, and isert them in the lists of squares to recolor.
        foreach (Transform t in square_to_color)
        {
            renderer = t.GetComponent<Renderer>();

            // materials in the script have an " (Instance)" at the end of the name. 
            if (renderer.material.name == this.yellow_board.name + " (Instance)")
            {
                this.yellow_square_colored_orange.Add(t);
                renderer.material = orange_color_1;
            }
            else
            {
                this.green_square_colored_orange.Add(t);
                renderer.material = orange_color_2;
            }

        }

    }

    // return true if the movement of the moving_piece in the destination_square dosen't put the king under check from the opponent.
    bool not_king_check(string king_square, Transform destination_square, Piece moving_piece)
    {
        bool king_threatened;
        string temp_square;
        int number_square;
        Piece temporary_eliminated=null;

        // find out the name of the destination square.
        temp_square = destination_square.parent.name;

        // find if there is another piece on the arriving square.
        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece piece = this.pieces_list[i];

            if (temp_square == piece.getSquareName())
            {
                temporary_eliminated = piece;
            }

        }

        // set the piece which is currently in the arriving square as "eliminated".
        if (temporary_eliminated != null)
        {
            temporary_eliminated.set_alive_status(false);
        }

        // get the number of the destination square.
        temp_square = temp_square.Substring(temp_square.Length - 2);
        number_square = int.Parse(temp_square);

        // store the original position of the piece.
        Transform original_square = moving_piece.get_position_square();

        // move the piece.
        moving_piece.move_to_square(number_square);

        // check if the king is under attack.
        king_threatened=check_square_under_attack(king_square);

        // get the number of the starting square.
        temp_square = original_square.parent.name;
        temp_square = temp_square.Substring(temp_square.Length - 2);
        number_square = int.Parse(temp_square);

        // restore the original position of the piece.
        moving_piece.move_to_square(number_square);

        // set the piece which is currently in the arriving square as not "eliminated".
        if (temporary_eliminated != null)
        {
            temporary_eliminated.set_alive_status(true);
        }

        return !(king_threatened);

    }

    // return true if the square is under attack from a enemie piece.
    // If the boolean value in input is true it means that we are checking if white pieces are attacking the square, if false we are cheking for the black ones.
    bool check_square_under_attack(string square_name)
    {

        bool check_attack_from_white_piece = !this.white_turn;

        for (int i = 0; i < pieces_list.Count; i++)
        {
            Piece attacking_piece = this.pieces_list[i];
            if (attacking_piece.get_alive_status())
            {

                if (attacking_piece.colorWhite() == check_attack_from_white_piece)
                {
                    int current_square_number;
                    Transform destination_square;

                    int iterator;
                    bool stop;

                    switch (attacking_piece.getPieceName())
                    {
                        case "pawn":

                            current_square_number = attacking_piece.get_square_number();

                            // if the pawn is white move up on the board if it is black it moves down.
                            if (attacking_piece.colorWhite())
                            {
                                if (current_square_number % 10 < 8)
                                {
                                    destination_square = GameObject.Find("board " + (current_square_number + 11)).transform;
                                    if (destination_square.name == square_name)
                                    {
                                        return true;
                                    }
                                }

                                if (current_square_number % 10 > 1)
                                {
                                    destination_square = GameObject.Find("board " + (current_square_number + 9)).transform;
                                    if (destination_square.name == square_name)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {

                                if (current_square_number % 10 < 8)
                                {
                                    destination_square = GameObject.Find("board " + (current_square_number - 9)).transform;
                                    if (destination_square.name == square_name)
                                    {
                                        return true;
                                    }
                                }

                                if (current_square_number % 10 > 1)
                                {
                                    destination_square = GameObject.Find("board " + (current_square_number - 11)).transform;
                                    if (destination_square.name == square_name)
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;

                        case "bishop":

                            current_square_number = attacking_piece.get_square_number();
                            iterator = 11;
                            stop = false;

                            while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator + 11;
                            }

                            iterator = 9;
                            stop = false;

                            while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator + 9;
                            }

                            iterator = -9;
                            stop = false;

                            while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator - 9;
                            }

                            iterator = -11;
                            stop = false;

                            while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator - 11;
                            }

                            break;

                        case "rook":

                            current_square_number = attacking_piece.get_square_number();
                            iterator = 1;
                            stop = false;

                            while (((current_square_number + iterator) % 10 <= 8) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator + 1;
                            }

                            iterator = -1;
                            stop = false;

                            while (((current_square_number + iterator) % 10 >= 1) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator - 1;
                            }

                            iterator = 10;
                            stop = false;

                            while (((current_square_number + iterator) / 10 <= 8) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator + 10;
                            }

                            iterator = -10;
                            stop = false;

                            while (((current_square_number + iterator) / 10 >= 1) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator - 10;
                            }

                            break;

                        case "queen":

                            current_square_number = attacking_piece.get_square_number();
                            iterator = 11;
                            stop = false;

                            while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator + 11;
                            }

                            iterator = 9;
                            stop = false;

                            while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator + 9;
                            }

                            iterator = -9;
                            stop = false;

                            while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator - 9;
                            }

                            iterator = -11;
                            stop = false;

                            while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }

                                iterator = iterator - 11;
                            }

                            iterator = 1;
                            stop = false;

                            while (((current_square_number + iterator) % 10 <= 8) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator + 1;
                            }

                            iterator = -1;
                            stop = false;

                            while (((current_square_number + iterator) % 10 >= 1) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator - 1;
                            }

                            iterator = 10;
                            stop = false;

                            while (((current_square_number + iterator) / 10 <= 8) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);

                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator + 10;
                            }

                            iterator = -10;
                            stop = false;

                            while (((current_square_number + iterator) / 10 >= 1) && !stop)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                                if (check_piece_presence(destination_square) && (!check_own_king_presence(destination_square)))
                                {
                                    stop = true;
                                }
                                if (destination_square.transform.parent.name == square_name)
                                {
                                    return true;
                                }
                                iterator = iterator - 10;
                            }

                            break;

                        case "king":

                            current_square_number = attacking_piece.get_square_number();

                            iterator = 1;
                            if ((current_square_number + iterator) % 10 <= 8)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = -1;
                            if ((current_square_number + iterator) % 10 >= 1)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = -10;
                            if ((current_square_number + iterator) / 10 >= 1)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = 10;
                            if ((current_square_number + iterator) / 10 <= 8)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = 11;
                            if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 <= 8))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = 9;
                            if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = -9;
                            if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = -11;
                            if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 >= 1))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            break;

                        case "knight":
                            current_square_number = attacking_piece.get_square_number();

                            iterator = 21;
                            if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 <= 8))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = 12;
                            if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = 19;
                            if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = 8;
                            if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = -8;
                            if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = -19;
                            if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = -21;
                            if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 >= 1))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            iterator = -12;
                            if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform;
                                if (destination_square.name == square_name)
                                {
                                    return true;
                                }
                            }

                            break;
                    }
                }
            }
        }
        return false;
    }

    // trasform a pawn into a queen.
    public void setQueen()
    {
        GameObject new_piece;
        Transform square_position = this.piece_to_update.get_position_square();
        Destroy(this.piece_to_update.get_piece_GameObject());

        if (this.piece_to_update.colorWhite())
        {
            new_piece=Instantiate(this.white_queen_object, square_position.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(0f, 0f, 0f));
            this.piece_to_update.set_new_type("queen", new_piece);
        }
        else
        {
            new_piece = Instantiate(this.black_queen_object, square_position.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(0f, 0f, 0f));
            this.piece_to_update.set_new_type("queen", new_piece);
        }

        this.UI_pawn_upgrade.gameObject.SetActive(false);

        this.menu_is_open = false;

        // set original_material_blue_king_check to null, so in the next turn it will be checked again if the king is under attack.
        this.original_material_blue_king_check = null;
        king_in_chek_renderer = null;
        this.white_turn = !white_turn;
    }

    // trasform a pawn into a rook.
    public void setRook()
    {
        GameObject new_piece;
        Transform square_position = this.piece_to_update.get_position_square();
        Destroy(this.piece_to_update.get_piece_GameObject());

        if (this.piece_to_update.colorWhite())
        {
            new_piece = Instantiate(this.white_rook_object, square_position.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(0f, 0f, 0f));
            this.piece_to_update.set_new_type("rook", new_piece);
        }
        else
        {
            new_piece = Instantiate(this.black_rook_object, square_position.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(0f, 0f, 0f));
            this.piece_to_update.set_new_type("rook", new_piece);
        }

        this.UI_pawn_upgrade.gameObject.SetActive(false);

        this.menu_is_open = false;

        // set original_material_blue_king_check to null, so in the next turn it will be checked again if the king is under attack.
        this.original_material_blue_king_check = null;
        king_in_chek_renderer = null;
        this.white_turn = !white_turn;
    }

    // trasform a pawn into a knight.
    public void setKnight()
    {
        GameObject new_piece;
        Transform square_position = this.piece_to_update.get_position_square();
        Destroy(this.piece_to_update.get_piece_GameObject());

        if (this.piece_to_update.colorWhite())
        {
            new_piece = Instantiate(this.white_knight_object, square_position.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(0f, 90f, 0f));
            this.piece_to_update.set_new_type("knight", new_piece);
        }
        else
        {
            new_piece = Instantiate(this.black_knight_object, square_position.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(0f, 270f, 0f));
            this.piece_to_update.set_new_type("knight", new_piece);
        }

        this.UI_pawn_upgrade.gameObject.SetActive(false);

        this.menu_is_open = false;

        // set original_material_blue_king_check to null, so in the next turn it will be checked again if the king is under attack.
        this.original_material_blue_king_check = null;
        king_in_chek_renderer = null;
        this.white_turn = !white_turn;
    }

    public void setBishop()
    {
        GameObject new_piece;
        Transform square_position = this.piece_to_update.get_position_square();
        Destroy(this.piece_to_update.get_piece_GameObject());

        if (this.piece_to_update.colorWhite())
        {
            new_piece = Instantiate(this.white_bishop_object, square_position.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(0f, 180f, 0f));
            this.piece_to_update.set_new_type("bishop", new_piece);
        }
        else
        {
            new_piece = Instantiate(this.black_bishop_object, square_position.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(0f, 0f, 0f));
            this.piece_to_update.set_new_type("bishop", new_piece);
        }

        this.UI_pawn_upgrade.gameObject.SetActive(false);

        this.menu_is_open = false;

        // set original_material_blue_king_check to null, so in the next turn it will be checked again if the king is under attack.
        this.original_material_blue_king_check = null;
        king_in_chek_renderer = null;
        this.white_turn = !white_turn;
    }

    public void menu()
    {
            this.UI_game_ended.gameObject.SetActive(true);
            this.menu_is_open = true;
    }

    public void menu_end()
    {
        this.UI_game_realy_ended.gameObject.SetActive(true);
        this.menu_is_open = true;
    }

    public void camera_menu()
    {
        this.UI_camera_menu.gameObject.SetActive(true);
        this.menu_is_open = true;
    }

    public void setCameraA()
    {
        this.cameraB.enabled = false;
        this.cameraC.enabled = false;
        this.cameraD.enabled = false;
        this.cameraE.enabled = false;
        this.cameraF.enabled = false;
        this.cameraA.enabled = true;

        this.UI_camera_menu.gameObject.SetActive(false);
        this.menu_is_open = false;
    }

    public void setCameraB()
    {
        this.cameraA.enabled = false;
        this.cameraC.enabled = false;
        this.cameraD.enabled = false;
        this.cameraE.enabled = false;
        this.cameraF.enabled = false;
        this.cameraB.enabled = true;

        this.UI_camera_menu.gameObject.SetActive(false);
        this.menu_is_open = false;
    }

    public void setCameraC()
    {
        this.cameraC.enabled = false;
        this.cameraB.enabled = false;
        this.cameraD.enabled = false;
        this.cameraE.enabled = false;
        this.cameraF.enabled = false;
        this.cameraC.enabled = true;

        this.UI_camera_menu.gameObject.SetActive(false);
        this.menu_is_open = false;
    }

    public void setCameraD()
    {
        this.cameraA.enabled = false;
        this.cameraB.enabled = false;
        this.cameraC.enabled = false;
        this.cameraE.enabled = false;
        this.cameraF.enabled = false;
        this.cameraD.enabled = true;

        this.UI_camera_menu.gameObject.SetActive(false);
        this.menu_is_open = false;
    }

    public void setCameraE()
    {
        this.cameraA.enabled = false;
        this.cameraB.enabled = false;
        this.cameraC.enabled = false;
        this.cameraD.enabled = false;
        this.cameraE.enabled = true;
        this.cameraF.enabled = false;

        this.UI_camera_menu.gameObject.SetActive(false);
        this.menu_is_open = false;
    }

    public void setCameraF()
    {
        this.cameraA.enabled = false;
        this.cameraB.enabled = false;
        this.cameraC.enabled = false;
        this.cameraD.enabled = false;
        this.cameraE.enabled = false;
        this.cameraF.enabled = true;

        this.UI_camera_menu.gameObject.SetActive(false);
        this.menu_is_open = false;
    }

    public void continue_the_game()
    {
        this.UI_game_ended.gameObject.SetActive(false);
        this.UI_game_realy_ended.gameObject.SetActive(false);
        this.UI_fifty_message.gameObject.SetActive(false);
        this.menu_is_open = false;
    }

    public void settings()
    {
        this.UI_game_ended.gameObject.SetActive(false);
        this.UI_settings_menu.gameObject.SetActive(true);
    }

    public void menu_fifty()
    {
        this.UI_fifty_message.gameObject.SetActive(true);
        this.menu_is_open = true;
    }

    public void allow_seventyfive_rule()
    {
        this.n_moves_implemented = true;

        this.UI_settings_menu.gameObject.SetActive(false);
        this.UI_game_ended.gameObject.SetActive(true);
    }

    public void disallow_seventyfive_rule()
    {
        this.n_moves_implemented = false;

        this.UI_settings_menu.gameObject.SetActive(false);
        this.UI_game_ended.gameObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene("board");
    }

    // this method define the movement of the pieces.
    public IEnumerator movement_routine(Piece piece, Transform starting_square, Transform destination_square)
    {
        // the constant values are the possible height reaceable in the movement of the pieces.
        const float no_obstacles_movement=6.5f;
        const float obstacles_movement = 9f;
        const float horse_obstacles = 18.5f;

        string name_square;
        int iterator;

        // first_obstacle_square indicate the "distance" from the starting square of the first obstacle.
        // last_obstacle_square indicate the "distance" from the starting square of the last obstacle.
        int first_obstacle_square = -1;
        int last_obstacle_square = -1;

        float height_movement = no_obstacles_movement;

        // fix the coordinate system of the plano on which draw the Bezier curve.

        // find the origin.
        Vector3 origin_position = starting_square.transform.position;
        Vector3 destination_position = destination_square.transform.position;

        origin_position = origin_position + new Vector3(0, 0.5f, 0);
        destination_position = destination_position + new Vector3(0, 0.5f, 0);

        // find the length of the vector that send origin_position vector in destination_position vector.
        Vector3 difference_vector = destination_position - origin_position;
        float difference_vector_length = difference_vector.magnitude;

        // find the vector distant 1 from the origin_point and on the line that contains the two points, origin and destination (on the xz plane).
        Vector3 first_base_vector = difference_vector*(1f / difference_vector_length);

        // find the vector distant 1 from the origin_point and perpendicular to the xz plane.
        Vector3 second_base_vector = new Vector3(0, 1f, 0);

        // get the number and the name that identify the destination square.
        name_square = destination_square.parent.name;
        int number_destination_square = int.Parse(name_square.Substring(name_square.Length - 2));

        // get the number and the name that identify the origin square.
        name_square = starting_square.parent.name;
        int number_starting_square = int.Parse(name_square.Substring(name_square.Length - 2));

        switch (piece.getPieceName())
        {
            case "tower":
                
                // the tower never has obstacles: do notthing.

                break;

            case "pawn":

                // obstacles possible only if the movement is not along the same coloumn.
                // If the pawn moves diagonaly, the possible obstacles are only the pieces on the two squares that are adjacent to both the starting square and the arriving square. 
                if (number_destination_square % 10 != number_starting_square % 10 && (check_piece_presence((number_destination_square / 10)*10 + number_starting_square % 10) || check_piece_presence((number_starting_square / 10)*10 + number_destination_square % 10))) 
                {
                    height_movement = obstacles_movement;
                }

                break;

            case "king":
                // obstacles possible only if the movement is not along the same coloumn.
                // As for the pawn, if the king moves diagonaly, the possible obstacles are only the pieces on the two squares that are adjacent to both the starting square and the arriving square. 
                if (number_destination_square % 10 != number_starting_square % 10 && (check_piece_presence((number_destination_square / 10)*10 + number_starting_square % 10) || check_piece_presence((number_starting_square / 10)*10 + number_destination_square % 10)))
                {
                    height_movement = obstacles_movement;
                }

                break;

            case "knight":

                // We consider the movement of the kinight as a combination of a diagonal move plus another movement along the coloumn or the row.
                // There is an obstacle if a piece is present on the square of the diagonal movement, or on the two squares that are adjacent to both the starting square and this diagonal square, plus the square, near the destination square, on the same row/coloumn of the starting square.

                int diagonal_square;
                // square near the destination square on the same row/coloumn of the starting square.
                int additional_square = 0;

                // Find out the diagonal square (and the additional one).
                if (number_destination_square/10 > number_starting_square / 10)
                {
                    diagonal_square = ((number_starting_square / 10) + 1)*10;

                    if(number_destination_square/10>(number_starting_square / 10 + 1))
                    {
                        additional_square = 20;
                    }
                }
                else
                {
                    diagonal_square = ((number_starting_square / 10) - 1)*10;

                    if (number_destination_square / 10 < (number_starting_square / 10 - 1))
                    {
                        additional_square = -20;
                    }
                }

                if (number_destination_square % 10 > number_starting_square % 10)
                {
                    diagonal_square = diagonal_square + number_starting_square % 10 + 1;

                    if (number_destination_square % 10 > (number_starting_square % 10 + 1))
                    {
                        additional_square = 2;
                    }
                }
                else
                {
                    diagonal_square = diagonal_square + number_starting_square % 10 - 1;

                    if (number_destination_square % 10 < (number_starting_square % 10 - 1))
                    {
                        additional_square = -2;
                    }
                }

                // check if a piece is on the diagonal square.
                if (check_piece_presence(diagonal_square))
                {
                    height_movement = horse_obstacles;
                }

                // check if a piece is on the additional square.
                if (check_piece_presence(number_starting_square+additional_square))
                {
                    height_movement = horse_obstacles;
                }

                // the check on the other two squares is performed in the same way of the king and the pawn cases.
                if (check_piece_presence((number_starting_square / 10)*10 + diagonal_square % 10) || check_piece_presence((diagonal_square / 10)*10 + number_starting_square % 10))
                {
                    height_movement = horse_obstacles;
                }

                break;

            case "bishop":

                if (number_destination_square / 10 > number_starting_square / 10)
                {
                    if (number_destination_square % 10 > number_starting_square % 10)
                    {
                        iterator = 0;
                        while(number_destination_square!=number_starting_square+iterator*11)
                        {
                            // there can be pieces only on the sqares adjacent those of the diagonal, not on the diagonal itself.
                            if ((check_piece_presence(number_starting_square +11*iterator+1) || check_piece_presence(number_starting_square + 11*iterator+10)))
                            {
                                last_obstacle_square = iterator;

                                if (first_obstacle_square == -1)
                                {
                                    first_obstacle_square = iterator+1;
                                }
                            }
                            iterator++;
                        }

                        last_obstacle_square = iterator - last_obstacle_square;
                    }
                    else
                    {
                        iterator = 0;
                        while (number_destination_square != number_starting_square + iterator * 9)
                        {
                            // there can be pieces only on the sqares adjacent those of the diagonal, not on the diagonal itself.
                            if ((check_piece_presence(number_starting_square + 9 * iterator - 1) || check_piece_presence(number_starting_square + 9 * iterator + 10)))
                            {
                                last_obstacle_square = iterator;

                                if (first_obstacle_square == -1)
                                {
                                    first_obstacle_square = iterator+1;
                                }
                            }
                            iterator++;
                        }
                        
                        last_obstacle_square = iterator - last_obstacle_square;
                    }
                }
                else
                {
                    if (number_destination_square % 10 > number_starting_square % 10)
                    {

                        iterator = 0;
                        while (number_destination_square != number_starting_square - iterator * 9)
                        {
                            // there can be pieces only on the sqares adjacent those of the diagonal, not on the diagonal itself.
                            if ((check_piece_presence(number_starting_square - 9 * iterator + 1) || check_piece_presence(number_starting_square - 9 * iterator - 10)))
                            {
                                last_obstacle_square = iterator;

                                if (first_obstacle_square == -1)
                                {
                                    first_obstacle_square = iterator+1;
                                }
                            }
                            iterator++;
                        }

                        last_obstacle_square = iterator - last_obstacle_square;
                    }
                    else
                    {
                        iterator = 0;
                        while (number_destination_square != number_starting_square - iterator * 11)
                        {
                            // there can be pieces only on the sqares adjacent those of the diagonal, not on the diagonal itself.
                            if ((check_piece_presence(number_starting_square - 11 * iterator - 1) || check_piece_presence(number_starting_square - 11 * iterator - 10)))
                            {
                                last_obstacle_square = iterator;

                                if (first_obstacle_square == -1)
                                {
                                    first_obstacle_square = iterator+1;
                                }
                            }
                            iterator++;
                        }
                       
                        last_obstacle_square = iterator - last_obstacle_square;
                    }
                }
                break;

            case "queen":

                if ((number_destination_square / 10 != number_starting_square / 10) && (number_destination_square % 10 != number_starting_square % 10))
                {
                    if (number_destination_square / 10 > number_starting_square / 10)
                    {
                        if (number_destination_square % 10 > number_starting_square % 10)
                        {
                            iterator = 0;
                            while (number_destination_square != number_starting_square + iterator * 11)
                            {
                                // there can be pieces only on the sqares adjacent those of the diagonal, not on the diagonal itself.
                                if ((check_piece_presence(number_starting_square + 11 * iterator + 1) || check_piece_presence(number_starting_square + 11 * iterator + 10)))
                                {
                                    last_obstacle_square = iterator;

                                    if (first_obstacle_square == -1)
                                    {
                                        first_obstacle_square = iterator+1;
                                    }
                                }
                                iterator++;
                            }

                            last_obstacle_square = iterator - last_obstacle_square;
                        }
                        else
                        {
                            iterator = 0;
                            while (number_destination_square != number_starting_square + iterator * 9)
                            {
                                // there can be pieces only on the sqares adjacent those of the diagonal, not on the diagonal itself.
                                if ((check_piece_presence(number_starting_square + 9 * iterator - 1) || check_piece_presence(number_starting_square + 9 * iterator + 10)))
                                {
                                    last_obstacle_square = iterator;

                                    if (first_obstacle_square == -1)
                                    {
                                        first_obstacle_square = iterator+1;
                                    }
                                }
                                iterator++;
                            }
                            last_obstacle_square = iterator - last_obstacle_square;
                        }
                    }
                    else
                    {
                        if (number_destination_square % 10 > number_starting_square % 10)
                        {

                            iterator = 0;
                            while (number_destination_square != number_starting_square - iterator * 9)
                            {
                                // there can be pieces only on the sqares adjacent those of the diagonal, not on the diagonal itself.
                                if ((check_piece_presence(number_starting_square - 9 * iterator + 1) || check_piece_presence(number_starting_square - 9 * iterator - 10)))
                                {
                                    last_obstacle_square = iterator;

                                    if (first_obstacle_square == -1)
                                    {
                                        first_obstacle_square = iterator+1;
                                    }
                                }
                                iterator++;
                            }

                            last_obstacle_square = iterator - last_obstacle_square;
                        }
                        else
                        {
                            iterator = 0;
                            while (number_destination_square != number_starting_square - iterator * 11)
                            {
                                // there can be pieces only on the sqares adjacent those of the diagonal, not on the diagonal itself.
                                if ((check_piece_presence(number_starting_square - 11 * iterator - 1) || check_piece_presence(number_starting_square - 11 * iterator - 10)))
                                {
                                    last_obstacle_square = iterator;

                                    if (first_obstacle_square == -1)
                                    {
                                        first_obstacle_square = iterator+1;
                                    }
                                }
                                iterator++;
                            }

                            last_obstacle_square = iterator - last_obstacle_square;
                        }
                    }
                }
                
                break;
        }

        if (first_obstacle_square == -1)
        {

            for (int i = 0; i <= 20; i++)
            {
                Vector3 moving_point = draw_bezier_curve_second(i / 20f, difference_vector_length, origin_position, first_base_vector, second_base_vector, height_movement);

                piece.move_to_point(moving_point);

                yield return new WaitForSecondsRealtime(0.02f);
            }
        }
        else
        {
            height_movement = obstacles_movement;

            for (int i = 0; i <= 20; i++)
            {
                Vector3 moving_point = draw_bezier_curve_third(i / 20f, difference_vector_length, origin_position, first_base_vector, second_base_vector, height_movement, first_obstacle_square, last_obstacle_square);

                piece.move_to_point(moving_point);

                yield return new WaitForSecondsRealtime(0.02f);
            }
        }

        piece.correct_movement_errors(number_destination_square);

    }

    // define the movement of the king and the rook when performing a castle move.
    public IEnumerator castle_movement_routine(Piece king_piece, Transform king_starting_square, Transform king_destination_square, Piece rook_piece, Transform rook_starting_square, Transform rook_destination_square)
    {
        // the constant values are the possible height reaceable in the movement of the pieces.
        const float height_rook_movement = 6.5f;
        const float height_king_movement = 18.5f;

        // origin_position and destination_position are the point, in the space, of the starting and of the arriving point of the movement.
        Vector3 origin_position;
        Vector3 destination_position;

        // difference_vector is the difference between the two previous vectors.
        Vector3 difference_vector;

        // first_base_vector and second_base_vector are the vectors that define the plane on which the bezier curve will be drawn.
        Vector3 first_base_vector;
        Vector3 second_base_vector;


        string name_square;

        // fix the coordinate system of the piano on which draw the Bezier curve for the rook movement.

        // find the origin.
        origin_position = rook_starting_square.transform.position;
        destination_position = rook_destination_square.transform.position;

        origin_position = origin_position + new Vector3(0, 0.5f, 0);
        destination_position = destination_position + new Vector3(0, 0.5f, 0);

        // find the length of the vector that send origin_position vector in destination_position vector.
        difference_vector = destination_position - origin_position;
        float difference_vector_length = difference_vector.magnitude;

        // find the vector distant 1 from the origin_point and on the line that contains the two points, origin and destination (on the xz plane).
        first_base_vector = difference_vector * (1f / difference_vector_length);

        // find the vector distant 1 from the origin_point and perpendicular to the xz plane.
        second_base_vector = new Vector3(0, 1f, 0);

        // get the number and the name that identify the destination square.
        name_square = rook_destination_square.parent.name;
        int number_destination_square = int.Parse(name_square.Substring(name_square.Length - 2));

        // get the number and the name that identify the origin square.
        name_square = rook_starting_square.parent.name;
        int number_starting_square = int.Parse(name_square.Substring(name_square.Length - 2));

        // draw the bezier curve.
        for (int i = 0; i <= 20; i++)
        {
            Vector3 moving_point = draw_bezier_curve_second(i / 20f, difference_vector_length, origin_position, first_base_vector, second_base_vector, height_rook_movement);

            rook_piece.move_to_point(moving_point);

            yield return new WaitForSecondsRealtime(0.02f);
        }


        // fix the coordinate system of the piano on which draw the Bezier curve for the king movement.

        // find the origin.
        origin_position = king_starting_square.transform.position;
        destination_position = king_destination_square.transform.position;

        origin_position = origin_position + new Vector3(0, 0.5f, 0);
        destination_position = destination_position + new Vector3(0, 0.5f, 0);

        // find the length of the vector that send origin_position vector in destination_position vector.
        difference_vector = destination_position - origin_position;
        difference_vector_length = difference_vector.magnitude;

        // find the vector distant 1 from the origin_point and on the line that contains the two points, origin and destination (on the xz plane).
        first_base_vector = difference_vector * (1f / difference_vector_length);

        // find the vector distant 1 from the origin_point and perpendicular to the xz plane.
        second_base_vector = new Vector3(0, 1f, 0);

        // get the number and the name that identify the destination square.
        name_square = king_destination_square.parent.name;
        number_destination_square = int.Parse(name_square.Substring(name_square.Length - 2));

        // get the number and the name that identify the origin square.
        name_square = king_starting_square.parent.name;
        number_starting_square = int.Parse(name_square.Substring(name_square.Length - 2));

        for (int i = 0; i <= 20; i++)
        {
            Vector3 moving_point = draw_bezier_curve_third(i / 20f, difference_vector_length, origin_position, first_base_vector, second_base_vector, height_king_movement, 1, 0);

            king_piece.move_to_point(moving_point);

            yield return new WaitForSecondsRealtime(0.02f);
        }

    }

    // draw the bezier curve on a plane, and find out the coordinate of the point of the curve in the space. 
    public Vector3 draw_bezier_curve_second(float t, float destination, Vector3 origin, Vector3 first_direction, Vector3 second_direction, float height)
    {
        // select the three point that define the curve.
        Vector2 point_zero = new Vector2(0, 0);
        Vector2 point_one = new Vector2(destination / 2f, height);
        Vector2 point_two = new Vector2(destination, 0);

        Vector2 first_intermediate_point = point_zero * (1 - t) + point_one * t;
        Vector2 second_intermediate_point = point_one * (1 - t) + point_two * t;

        Vector2 result_point = first_intermediate_point * (1 - t) + second_intermediate_point * t;

        return origin + (result_point.x*first_direction) + (result_point.y*second_direction);
    }

    // draw the bezier curve on a plane, and find out the coordinate of the point of the curve in the space. 
    public Vector3 draw_bezier_curve_third(float t, float destination, Vector3 origin, Vector3 first_direction, Vector3 second_direction, float height, int first_obstacle_distance, int last_obstacle_distance)
    {
        // four point that define the curve.
        Vector2 point_zero = new Vector2(0, 0);
        Vector2 point_one;
        Vector2 point_two;
        Vector2 point_three = new Vector2(destination, 0);

        if (first_obstacle_distance > 3) 
        {
            point_one = new Vector2(destination * 0.035f,height);
        }
        else
        {
            point_one = new Vector2(destination * (first_obstacle_distance/100f+0.005f), height);
        }

        if (last_obstacle_distance > 3)
        {
            point_two = new Vector2(destination * 0.065f, height);
        }
        else
        {
            point_two = new Vector2(destination * (1 -last_obstacle_distance/100f-0.005f), height);
        }

        Vector2 first_intermediate_point = point_zero * (1 - t) + point_one * t;
        Vector2 second_intermediate_point = point_one * (1 - t) + point_two * t;
        Vector2 third_intermediate_point = point_two * (1 - t) + point_three * t;

        // reuse the variables of the first iteration of the De Casteljau's algorithm for the points of the second iteration.

        first_intermediate_point = first_intermediate_point * (1 - t) + second_intermediate_point * t;
        second_intermediate_point = second_intermediate_point * (1 - t) + third_intermediate_point * t;

        Vector2 result_point = first_intermediate_point * (1 - t) + second_intermediate_point * t;

        return origin + (result_point.x * first_direction) + (result_point.y * second_direction);
    }

    public bool check_finished_game()
    {

        foreach(Piece piece_to_move in this.pieces_list) {

            if (this.white_turn == piece_to_move.colorWhite())
            {

                // find out the square of the king.
                string king_square = null;

                string square_name = piece_to_move.getSquareName();


                for (int i = 0; i < pieces_list.Count; i++)
                {
                    Piece piece = this.pieces_list[i];

                    if (piece.getPieceName() == "king" && (this.white_turn == piece.colorWhite()))
                    {
                        king_square = piece.getSquareName();
                    }
                }

                // if a move is possible return false, else return true.

                int current_square_number;
                Transform destination_square;

                int iterator;
                bool stop;

                switch (piece_to_move.getPieceName())
                {
                    case "pawn":

                        // if the pawn is white move up on the board if it is black it moves down.
                        if (piece_to_move.colorWhite())
                        {
                            // move forward.
                            current_square_number = piece_to_move.get_square_number();
                            destination_square = GameObject.Find("board " + (current_square_number + 10)).transform.GetChild(0);

                            if (!check_piece_presence(destination_square) && not_king_check(king_square, destination_square, piece_to_move))
                            {
                                return false;
                            }

                            if (!check_piece_presence(destination_square) && piece_to_move.has_never_moved_before())
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + 20)).transform.GetChild(0);

                                if (!check_piece_presence(destination_square) && not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }

                            // capture a pieces.
                            if (current_square_number % 10 < 8)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + 11)).transform.GetChild(0);
                                if (check_movable_piece(destination_square, false) && not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }

                                if ((current_square_number + 11) == this.special_move_square && not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }

                            if (current_square_number % 10 > 1)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number + 9)).transform.GetChild(0);
                                if (check_movable_piece(destination_square, false) && not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }

                                if ((current_square_number + 9) == this.special_move_square && not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            // move forward.
                            current_square_number = piece_to_move.get_square_number();
                            destination_square = GameObject.Find("board " + (current_square_number - 10)).transform.GetChild(0);

                            if (!check_piece_presence(destination_square) && not_king_check(king_square, destination_square, piece_to_move))
                            {
                                return false;
                            }

                            if (!check_piece_presence(destination_square) && piece_to_move.has_never_moved_before())
                            {
                                destination_square = GameObject.Find("board " + (current_square_number - 20)).transform.GetChild(0);

                                if (!check_piece_presence(destination_square) && not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }

                            // capture a pieces.
                            if (current_square_number % 10 < 8)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number - 9)).transform.GetChild(0);
                                if (check_movable_piece(destination_square, true) && not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }

                                if ((current_square_number - 9) == this.special_move_square && not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }

                            if (current_square_number % 10 > 1)
                            {
                                destination_square = GameObject.Find("board " + (current_square_number - 11)).transform.GetChild(0);
                                if (check_movable_piece(destination_square, true) && not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }

                                if ((current_square_number - 11) == this.special_move_square && not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                        }
                        break;

                    case "bishop":

                        current_square_number = piece_to_move.get_square_number();
                        iterator = 11;
                        stop = false;

                        while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator + 11;
                        }

                        iterator = 9;
                        stop = false;

                        while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator + 9;
                        }

                        iterator = -9;
                        stop = false;

                        while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator - 9;
                        }

                        iterator = -11;
                        stop = false;

                        while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator - 11;
                        }

                        break;

                    case "rook":

                        current_square_number = piece_to_move.get_square_number();
                        iterator = 1;
                        stop = false;

                        while (((current_square_number + iterator) % 10 <= 8) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator + 1;
                        }

                        iterator = -1;
                        stop = false;

                        while (((current_square_number + iterator) % 10 >= 1) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator - 1;
                        }

                        iterator = 10;
                        stop = false;

                        while (((current_square_number + iterator) / 10 <= 8) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator + 10;
                        }

                        iterator = -10;
                        stop = false;

                        while (((current_square_number + iterator) / 10 >= 1) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator - 10;
                        }

                        break;

                    case "queen":

                        current_square_number = piece_to_move.get_square_number();
                        iterator = 11;
                        stop = false;

                        while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator + 11;
                        }

                        iterator = 9;
                        stop = false;

                        while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 <= 8) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator + 9;
                        }

                        iterator = -9;
                        stop = false;

                        while (((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator - 9;
                        }

                        iterator = -11;
                        stop = false;

                        while (((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) / 10 >= 1) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }

                            iterator = iterator - 11;
                        }

                        iterator = 1;
                        stop = false;

                        while (((current_square_number + iterator) % 10 <= 8) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator + 1;
                        }

                        iterator = -1;
                        stop = false;

                        while (((current_square_number + iterator) % 10 >= 1) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator - 1;
                        }

                        iterator = 10;
                        stop = false;

                        while (((current_square_number + iterator) / 10 <= 8) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator + 10;
                        }

                        iterator = -10;
                        stop = false;

                        while (((current_square_number + iterator) / 10 >= 1) && !stop)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_piece_presence(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                stop = true;
                                if (!check_movable_piece(destination_square))
                                {
                                    if (not_king_check(king_square, destination_square, piece_to_move))
                                    {
                                        return false;
                                    }
                                }
                            }
                            iterator = iterator - 10;
                        }

                        break;

                    case "king":

                        current_square_number = piece_to_move.get_square_number();

                        iterator = 1;
                        if ((current_square_number + iterator) % 10 <= 8)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                            {
                                return false;
                            }
                        }

                        iterator = -1;
                        if ((current_square_number + iterator) % 10 >= 1)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                            {
                                return false;
                            }
                        }

                        iterator = -10;
                        if ((current_square_number + iterator) / 10 >= 1)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                            {
                                return false;
                            }
                        }

                        iterator = 10;
                        if ((current_square_number + iterator) / 10 <= 8)
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                            {
                                return false;
                            }
                        }

                        iterator = 11;
                        if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 <= 8))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square, this.white_turn) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                            {
                                return false;
                            }
                        }

                        iterator = 9;
                        if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                            {
                                return false;
                            }
                        }

                        iterator = -9;
                        if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                            {
                                return false;
                            }
                        }

                        iterator = -11;
                        if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 >= 1))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square) && not_king_check(destination_square.parent.name, destination_square, piece_to_move))
                            {
                                return false;
                            }
                        }

                        break;

                    case "knight":
                        current_square_number = piece_to_move.get_square_number();

                        iterator = 21;
                        if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 <= 8))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                        }

                        iterator = 12;
                        if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                        }

                        iterator = 19;
                        if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                        }

                        iterator = 8;
                        if (((current_square_number + iterator) / 10 <= 8) && ((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                        }

                        iterator = -8;
                        if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 <= 8) && ((current_square_number + iterator) % 10 >= 1))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                        }

                        iterator = -19;
                        if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                        }

                        iterator = -21;
                        if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 >= 1))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                        }

                        iterator = -12;
                        if (((current_square_number + iterator) / 10 >= 1) && ((current_square_number + iterator) % 10 >= 1) && ((current_square_number + iterator) % 10 <= 8))
                        {
                            destination_square = GameObject.Find("board " + (current_square_number + iterator)).transform.GetChild(0);
                            if (!check_movable_piece(destination_square))
                            {
                                if (not_king_check(king_square, destination_square, piece_to_move))
                                {
                                    return false;
                                }
                            }
                        }

                        break;
                }

            }//if
        }//for

        return true;
    }
}